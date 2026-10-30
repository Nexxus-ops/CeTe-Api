"""Real HTTP acceptance checks. Creates disposable tenants, never prints credentials/tokens."""
import json
import os
import secrets
import urllib.request
import urllib.error
import uuid
from concurrent.futures import ThreadPoolExecutor
from threading import Barrier

BASE = os.environ.get('CETE_API_URL', 'http://127.0.0.1:5080/api/v1')

def request(method, path, body=None, token=None, expected=200):
    headers = {'Content-Type': 'application/json'}
    if token:
        headers['Authorization'] = 'Bearer ' + token
    req = urllib.request.Request(BASE + path, data=json.dumps(body).encode() if body is not None else None, headers=headers, method=method)
    try:
        with urllib.request.urlopen(req, timeout=20) as response:
            status, raw = response.status, response.read()
    except urllib.error.HTTPError as error:
        status, raw = error.code, error.read()
    assert status == expected, f'{method} {path}: expected {expected}, got {status}; {raw[:180] if status >= 400 else "response omitted"}'
    return json.loads(raw) if raw else None

def account():
    email = f'audit-{uuid.uuid4().hex}@example.invalid'
    password = secrets.token_urlsafe(20)
    ruc = '20' + str(secrets.randbelow(10**9)).zfill(9)
    request('POST', '/authentication/sign-up', {'username': email, 'password': password, 'companyName': 'Acceptance tenant', 'ruc': ruc, 'address': 'Test warehouse, Lima'})
    token = request('POST', '/authentication/sign-in', {'username': email, 'password': password})['token']
    return token, email

def run():
    token, email = account()
    other, _ = account()
    request('POST', '/authentication/sign-in', {'username': email, 'password': 'incorrect-password'}, expected=401)
    request('GET', '/inventory', expected=401)
    p = request('POST', '/inventory', {'sku': 'SKU-A', 'name': 'Original product', 'quantity': 0, 'unitPrice': 12.50}, token)
    q = request('POST', '/inventory', {'sku': 'SKU-B', 'name': 'Second product', 'quantity': 0, 'unitPrice': 5}, token)
    request('POST', '/inventory', {'sku': 'SKU-A', 'name': 'Duplicate', 'quantity': 0, 'unitPrice': 2}, token, 409)
    purchase = request('POST', '/purchases', {'supplierName': 'Test supplier', 'productId': p['id'], 'quantity': 10, 'unitCost': 8}, token)
    request('POST', f'/purchases/{purchase["id"]}/receive', {'quantity': 4}, token)
    request('POST', f'/purchases/{purchase["id"]}/receive', {'quantity': 7}, token, 422)
    request('POST', f'/purchases/{purchase["id"]}/receive', {'quantity': 6}, token)
    request('POST', f'/inventory/{q["id"]}/entries', {'quantity': 1, 'reason': 'TEST'}, token)
    stock = lambda: {item['id']: item['quantity'] for item in request('GET', '/inventory', token=token)}
    assert stock()[p['id']] == 10
    failing = {'id': str(uuid.uuid4()), 'customerName': 'Test', 'paymentMethod': 'CASH', 'items': [{'inventoryItemId': p['id'], 'quantity': 2}, {'inventoryItemId': q['id'], 'quantity': 2}]}
    request('POST', '/sales', failing, token, 409)
    assert stock()[p['id']] == 10, 'A failed sale must roll back every stock deduction'
    assert request('GET', '/sales', token=token) == []
    sale = {'id': str(uuid.uuid4()), 'customerName': 'Receipt customer', 'paymentMethod': 'CASH', 'items': [{'inventoryItemId': p['id'], 'quantity': 2}]}
    created = request('POST', '/sales', sale, token)
    request('POST', '/sales', sale, token)
    assert created['totalAmount'] == 25 and stock()[p['id']] == 8
    receipt = request('GET', f'/sales/{sale["id"]}/receipt', token=token)
    assert receipt['total'] == 25 and receipt['address'] == 'Test warehouse, Lima'
    request('PUT', f'/inventory/{p["id"]}', {'sku': 'SKU-A', 'name': 'Updated product', 'quantity': 8, 'unitPrice': 99}, token)
    original = request('GET', f'/sales/{sale["id"]}/receipt', token=token)
    assert original['items'][0]['name'] == 'Original product' and original['items'][0]['unitPrice'] == 12.50
    assert request('GET', '/inventory', token=other) == []
    request('GET', f'/sales/{sale["id"]}/receipt', token=other, expected=404)
    request('PATCH', f'/sales/{sale["id"]}/cancel', {}, token)
    request('PATCH', f'/sales/{sale["id"]}/cancel', {}, token, 409)
    assert stock()[p['id']] == 10, 'Cancellation must restore stock exactly once'
    employee_email = f'warehouse-{uuid.uuid4().hex}@example.invalid'
    employee_password = secrets.token_urlsafe(20)
    request('POST', '/users', {'username': employee_email, 'password': employee_password, 'role': 'Warehouse'}, token)
    employee = request('POST', '/authentication/sign-in', {'username': employee_email, 'password': employee_password})['token']
    request('POST', '/sales', {**sale, 'id': str(uuid.uuid4())}, employee, 403)
    request('POST', '/authentication/sign-out', {}, employee, 204)
    request('GET', '/inventory', token=employee, expected=401)
    scarce = request('POST', '/inventory', {'sku': 'LAST-UNIT', 'name': 'Last unit', 'quantity': 0, 'unitPrice': 7.50}, token)
    request('POST', f'/inventory/{scarce["id"]}/entries', {'quantity': 1, 'reason': 'CONCURRENCY TEST'}, token)
    barrier = Barrier(2)
    def competing_sale(_):
        body = {'id': str(uuid.uuid4()), 'customerName': 'Concurrent buyer', 'paymentMethod': 'CASH', 'items': [{'inventoryItemId': scarce['id'], 'quantity': 1}]}
        barrier.wait()
        req = urllib.request.Request(BASE + '/sales', data=json.dumps(body).encode(), headers={'Content-Type': 'application/json', 'Authorization': 'Bearer ' + token}, method='POST')
        try:
            with urllib.request.urlopen(req, timeout=20) as response: return response.status
        except urllib.error.HTTPError as error: return error.code
    with ThreadPoolExecutor(max_workers=2) as pool: statuses = list(pool.map(competing_sale, range(2)))
    assert sorted(statuses) == [200, 409], f'Expected one sale and one conflict, got {statuses}'
    assert stock()[scarce['id']] == 0, 'Concurrent sales must never oversell'
    active = lambda: request('POST', '/sales', {'id': str(uuid.uuid4()), 'customerName': 'Dispatch buyer', 'paymentMethod': 'CASH', 'items': [{'inventoryItemId': p['id'], 'quantity': 1}]}, token)
    first, second = active(), active()
    dispatch = request('POST', '/dispatches', {'driverName': 'Test driver', 'licensePlate': 'TEST-001', 'saleIds': [first['id']]}, token)
    request('POST', '/dispatches', {'driverName': 'Another driver', 'licensePlate': 'TEST-002', 'saleIds': [first['id']]}, token, 409)
    request('POST', '/dispatches', {'driverName': 'Test driver', 'licensePlate': 'TEST-001', 'saleIds': [second['id']]}, token, 409)
    request('PATCH', f'/sales/{first["id"]}/cancel', {}, token, 409)
    request('PUT', f'/dispatches/{dispatch["id"]}', {'status': 'DELIVERED'}, token, 409)
    request('PUT', f'/dispatches/{dispatch["id"]}', {'status': 'IN_TRANSIT'}, token)
    request('POST', f'/dispatches/{dispatch["id"]}/incidents', {'description': 'Road closed for test'}, token)
    request('PUT', f'/dispatches/{dispatch["id"]}', {'status': 'IN_TRANSIT'}, token)
    request('PUT', f'/dispatches/{dispatch["id"]}', {'status': 'DELIVERED'}, token)
    rate = request('GET', '/integrations/exchange-rates', token=token)
    assert rate['pen'] > 0 and rate['source'] == 'ExchangeRate-API'
    print('PASS: login rejection, authentication, tenant isolation, roles, duplicate SKU, partial receipts, stock rollback, idempotency, historical receipt, cancellation, session revocation.')
    print('PASS: simultaneous last-unit sales, exclusive dispatch sale, occupied resources, dispatch transitions/incidents and live external exchange rate.')

if __name__ == '__main__':
    run()
