--
-- PostgreSQL database dump
--

\restrict okFGRH4KHbV37eif6P7ldcre84Dp5FLAakwxOc8ZD7qygdo0TuNHsFjuPsRwoIZ

-- Dumped from database version 17.11
-- Dumped by pg_dump version 17.11

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Categories; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Categories" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: Customers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Customers" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Contact" text NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: DeliveryAttempts; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."DeliveryAttempts" (
    "Id" uuid NOT NULL,
    "DispatchStopId" uuid NOT NULL,
    "Outcome" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: DispatchIncidents; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."DispatchIncidents" (
    "Id" uuid NOT NULL,
    "DispatchId" uuid NOT NULL,
    "Description" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: DispatchSales; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."DispatchSales" (
    "Id" uuid NOT NULL,
    "DispatchId" uuid NOT NULL,
    "SaleId" uuid NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: DispatchStops; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."DispatchStops" (
    "Id" uuid NOT NULL,
    "DispatchId" uuid NOT NULL,
    "Address" text NOT NULL,
    "Sequence" integer NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: Dispatches; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Dispatches" (
    "Id" uuid NOT NULL,
    "DriverName" text NOT NULL,
    "LicensePlate" text NOT NULL,
    "Status" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: Drivers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Drivers" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "License" text NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: GoodsReceiptLines; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."GoodsReceiptLines" (
    "Id" uuid NOT NULL,
    "GoodsReceiptId" uuid NOT NULL,
    "PurchaseOrderLineId" uuid NOT NULL,
    "Quantity" numeric(18,2) NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: GoodsReceipts; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."GoodsReceipts" (
    "Id" uuid NOT NULL,
    "PurchaseOrderId" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: InventoryMovements; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."InventoryMovements" (
    "Id" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "WarehouseId" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Quantity" numeric(18,2) NOT NULL,
    "Reason" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: Locations; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Locations" (
    "Id" uuid NOT NULL,
    "WarehouseId" uuid NOT NULL,
    "Code" text NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: Notifications; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Notifications" (
    "Id" uuid NOT NULL,
    "Message" text NOT NULL,
    "EventKey" text NOT NULL,
    "IsRead" boolean NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: Payments; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Payments" (
    "Id" uuid NOT NULL,
    "SaleId" uuid NOT NULL,
    "Method" text NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "Status" text NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: ProductVariants; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ProductVariants" (
    "Id" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "Sku" text NOT NULL,
    "AttributesJson" text NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: Products; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Products" (
    "Id" uuid NOT NULL,
    "Sku" text NOT NULL,
    "Name" text NOT NULL,
    "UnitPrice" numeric(18,2) NOT NULL,
    "MinimumStock" numeric(18,2) NOT NULL,
    "Active" boolean NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: PurchaseOrderLines; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."PurchaseOrderLines" (
    "Id" uuid NOT NULL,
    "PurchaseOrderId" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "Quantity" numeric(18,2) NOT NULL,
    "UnitCost" numeric(18,2) NOT NULL,
    "ReceivedQuantity" numeric(18,2) NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: PurchaseOrders; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."PurchaseOrders" (
    "Id" uuid NOT NULL,
    "SupplierId" uuid NOT NULL,
    "WarehouseId" uuid NOT NULL,
    "Status" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: ReturnLines; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ReturnLines" (
    "Id" uuid NOT NULL,
    "ReturnId" uuid NOT NULL,
    "SaleLineId" uuid NOT NULL,
    "Quantity" numeric(18,2) NOT NULL,
    "Resolution" text NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: Returns; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Returns" (
    "Id" uuid NOT NULL,
    "SaleId" uuid NOT NULL,
    "Reason" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: Roles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Roles" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: SaleLines; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."SaleLines" (
    "Id" uuid NOT NULL,
    "SaleId" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "Sku" text NOT NULL,
    "Name" text NOT NULL,
    "Quantity" numeric(18,2) NOT NULL,
    "UnitPrice" numeric(18,2) NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: Sales; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Sales" (
    "Id" uuid NOT NULL,
    "CustomerName" text NOT NULL,
    "WarehouseId" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "TotalAmount" numeric(18,2) NOT NULL,
    "Status" text NOT NULL,
    "RequestHash" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: SalesReceipts; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."SalesReceipts" (
    "Id" uuid NOT NULL,
    "SaleId" uuid NOT NULL,
    "Number" text NOT NULL,
    "CompanyName" text NOT NULL,
    "Ruc" text NOT NULL,
    "Address" text NOT NULL,
    "Cashier" text NOT NULL,
    "IssuedAt" timestamp with time zone NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: StockBalances; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."StockBalances" (
    "Id" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "WarehouseId" uuid NOT NULL,
    "Quantity" numeric(18,2) NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: StockLots; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."StockLots" (
    "Id" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "Code" text NOT NULL,
    "ExpiresAt" timestamp with time zone,
    "TenantId" uuid NOT NULL
);


--
-- Name: SubscriptionPlans; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."SubscriptionPlans" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "MonthlyPrice" numeric(18,2) NOT NULL
);


--
-- Name: Suppliers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Suppliers" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Contact" text NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: TenantSubscriptions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."TenantSubscriptions" (
    "Id" uuid NOT NULL,
    "SubscriptionPlanId" uuid NOT NULL,
    "Status" text NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: Tenants; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Tenants" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Ruc" text NOT NULL,
    "Address" text NOT NULL
);


--
-- Name: UserRoles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserRoles" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "RoleId" uuid NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: UserSessions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserSessions" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "ExpiresAt" timestamp with time zone NOT NULL,
    "Revoked" boolean NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: Users; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Users" (
    "Id" uuid NOT NULL,
    "Username" text NOT NULL,
    "PasswordHash" text NOT NULL,
    "Role" text NOT NULL,
    "Active" boolean NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: Vehicles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Vehicles" (
    "Id" uuid NOT NULL,
    "LicensePlate" text NOT NULL,
    "Capacity" numeric(18,2) NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: Warehouses; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Warehouses" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Address" text NOT NULL,
    "TenantId" uuid NOT NULL
);


--
-- Name: Categories AK_Categorys_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Categories"
    ADD CONSTRAINT "AK_Categorys_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: Customers AK_Customers_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Customers"
    ADD CONSTRAINT "AK_Customers_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: DeliveryAttempts AK_DeliveryAttempts_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DeliveryAttempts"
    ADD CONSTRAINT "AK_DeliveryAttempts_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: DispatchIncidents AK_DispatchIncidents_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispatchIncidents"
    ADD CONSTRAINT "AK_DispatchIncidents_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: DispatchSales AK_DispatchSales_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispatchSales"
    ADD CONSTRAINT "AK_DispatchSales_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: DispatchStops AK_DispatchStops_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispatchStops"
    ADD CONSTRAINT "AK_DispatchStops_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: Dispatches AK_Dispatchs_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Dispatches"
    ADD CONSTRAINT "AK_Dispatchs_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: Drivers AK_Drivers_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Drivers"
    ADD CONSTRAINT "AK_Drivers_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: GoodsReceiptLines AK_GoodsReceiptLines_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."GoodsReceiptLines"
    ADD CONSTRAINT "AK_GoodsReceiptLines_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: GoodsReceipts AK_GoodsReceipts_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."GoodsReceipts"
    ADD CONSTRAINT "AK_GoodsReceipts_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: InventoryMovements AK_InventoryMovements_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InventoryMovements"
    ADD CONSTRAINT "AK_InventoryMovements_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: Locations AK_Locations_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Locations"
    ADD CONSTRAINT "AK_Locations_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: Notifications AK_Notifications_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Notifications"
    ADD CONSTRAINT "AK_Notifications_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: Payments AK_Payments_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Payments"
    ADD CONSTRAINT "AK_Payments_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: ProductVariants AK_ProductVariants_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ProductVariants"
    ADD CONSTRAINT "AK_ProductVariants_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: Products AK_Products_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Products"
    ADD CONSTRAINT "AK_Products_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: PurchaseOrderLines AK_PurchaseOrderLines_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PurchaseOrderLines"
    ADD CONSTRAINT "AK_PurchaseOrderLines_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: PurchaseOrders AK_PurchaseOrders_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PurchaseOrders"
    ADD CONSTRAINT "AK_PurchaseOrders_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: ReturnLines AK_ReturnLines_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ReturnLines"
    ADD CONSTRAINT "AK_ReturnLines_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: Returns AK_Returns_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Returns"
    ADD CONSTRAINT "AK_Returns_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: Roles AK_Roles_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Roles"
    ADD CONSTRAINT "AK_Roles_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: SaleLines AK_SaleLines_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SaleLines"
    ADD CONSTRAINT "AK_SaleLines_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: SalesReceipts AK_SalesReceipts_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SalesReceipts"
    ADD CONSTRAINT "AK_SalesReceipts_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: Sales AK_Sales_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Sales"
    ADD CONSTRAINT "AK_Sales_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: StockBalances AK_StockBalances_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockBalances"
    ADD CONSTRAINT "AK_StockBalances_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: StockLots AK_StockLots_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockLots"
    ADD CONSTRAINT "AK_StockLots_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: Suppliers AK_Suppliers_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Suppliers"
    ADD CONSTRAINT "AK_Suppliers_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: TenantSubscriptions AK_TenantSubscriptions_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."TenantSubscriptions"
    ADD CONSTRAINT "AK_TenantSubscriptions_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: UserRoles AK_UserRoles_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserRoles"
    ADD CONSTRAINT "AK_UserRoles_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: UserSessions AK_UserSessions_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserSessions"
    ADD CONSTRAINT "AK_UserSessions_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: Users AK_Users_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "AK_Users_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: Vehicles AK_Vehicles_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Vehicles"
    ADD CONSTRAINT "AK_Vehicles_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: Warehouses AK_Warehouses_TenantId_Id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Warehouses"
    ADD CONSTRAINT "AK_Warehouses_TenantId_Id" UNIQUE ("TenantId", "Id");


--
-- Name: Categories PK_Categorys; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Categories"
    ADD CONSTRAINT "PK_Categorys" PRIMARY KEY ("Id");


--
-- Name: Customers PK_Customers; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Customers"
    ADD CONSTRAINT "PK_Customers" PRIMARY KEY ("Id");


--
-- Name: DeliveryAttempts PK_DeliveryAttempts; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DeliveryAttempts"
    ADD CONSTRAINT "PK_DeliveryAttempts" PRIMARY KEY ("Id");


--
-- Name: DispatchIncidents PK_DispatchIncidents; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispatchIncidents"
    ADD CONSTRAINT "PK_DispatchIncidents" PRIMARY KEY ("Id");


--
-- Name: DispatchSales PK_DispatchSales; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispatchSales"
    ADD CONSTRAINT "PK_DispatchSales" PRIMARY KEY ("Id");


--
-- Name: DispatchStops PK_DispatchStops; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispatchStops"
    ADD CONSTRAINT "PK_DispatchStops" PRIMARY KEY ("Id");


--
-- Name: Dispatches PK_Dispatchs; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Dispatches"
    ADD CONSTRAINT "PK_Dispatchs" PRIMARY KEY ("Id");


--
-- Name: Drivers PK_Drivers; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Drivers"
    ADD CONSTRAINT "PK_Drivers" PRIMARY KEY ("Id");


--
-- Name: GoodsReceiptLines PK_GoodsReceiptLines; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."GoodsReceiptLines"
    ADD CONSTRAINT "PK_GoodsReceiptLines" PRIMARY KEY ("Id");


--
-- Name: GoodsReceipts PK_GoodsReceipts; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."GoodsReceipts"
    ADD CONSTRAINT "PK_GoodsReceipts" PRIMARY KEY ("Id");


--
-- Name: InventoryMovements PK_InventoryMovements; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InventoryMovements"
    ADD CONSTRAINT "PK_InventoryMovements" PRIMARY KEY ("Id");


--
-- Name: Locations PK_Locations; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Locations"
    ADD CONSTRAINT "PK_Locations" PRIMARY KEY ("Id");


--
-- Name: Notifications PK_Notifications; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Notifications"
    ADD CONSTRAINT "PK_Notifications" PRIMARY KEY ("Id");


--
-- Name: Payments PK_Payments; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Payments"
    ADD CONSTRAINT "PK_Payments" PRIMARY KEY ("Id");


--
-- Name: ProductVariants PK_ProductVariants; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ProductVariants"
    ADD CONSTRAINT "PK_ProductVariants" PRIMARY KEY ("Id");


--
-- Name: Products PK_Products; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Products"
    ADD CONSTRAINT "PK_Products" PRIMARY KEY ("Id");


--
-- Name: PurchaseOrderLines PK_PurchaseOrderLines; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PurchaseOrderLines"
    ADD CONSTRAINT "PK_PurchaseOrderLines" PRIMARY KEY ("Id");


--
-- Name: PurchaseOrders PK_PurchaseOrders; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PurchaseOrders"
    ADD CONSTRAINT "PK_PurchaseOrders" PRIMARY KEY ("Id");


--
-- Name: ReturnLines PK_ReturnLines; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ReturnLines"
    ADD CONSTRAINT "PK_ReturnLines" PRIMARY KEY ("Id");


--
-- Name: Returns PK_Returns; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Returns"
    ADD CONSTRAINT "PK_Returns" PRIMARY KEY ("Id");


--
-- Name: Roles PK_Roles; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Roles"
    ADD CONSTRAINT "PK_Roles" PRIMARY KEY ("Id");


--
-- Name: SaleLines PK_SaleLines; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SaleLines"
    ADD CONSTRAINT "PK_SaleLines" PRIMARY KEY ("Id");


--
-- Name: Sales PK_Sales; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Sales"
    ADD CONSTRAINT "PK_Sales" PRIMARY KEY ("Id");


--
-- Name: SalesReceipts PK_SalesReceipts; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SalesReceipts"
    ADD CONSTRAINT "PK_SalesReceipts" PRIMARY KEY ("Id");


--
-- Name: StockBalances PK_StockBalances; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockBalances"
    ADD CONSTRAINT "PK_StockBalances" PRIMARY KEY ("Id");


--
-- Name: StockLots PK_StockLots; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockLots"
    ADD CONSTRAINT "PK_StockLots" PRIMARY KEY ("Id");


--
-- Name: SubscriptionPlans PK_SubscriptionPlans; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SubscriptionPlans"
    ADD CONSTRAINT "PK_SubscriptionPlans" PRIMARY KEY ("Id");


--
-- Name: Suppliers PK_Suppliers; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Suppliers"
    ADD CONSTRAINT "PK_Suppliers" PRIMARY KEY ("Id");


--
-- Name: TenantSubscriptions PK_TenantSubscriptions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."TenantSubscriptions"
    ADD CONSTRAINT "PK_TenantSubscriptions" PRIMARY KEY ("Id");


--
-- Name: Tenants PK_Tenants; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Tenants"
    ADD CONSTRAINT "PK_Tenants" PRIMARY KEY ("Id");


--
-- Name: UserRoles PK_UserRoles; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserRoles"
    ADD CONSTRAINT "PK_UserRoles" PRIMARY KEY ("Id");


--
-- Name: UserSessions PK_UserSessions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserSessions"
    ADD CONSTRAINT "PK_UserSessions" PRIMARY KEY ("Id");


--
-- Name: Users PK_Users; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "PK_Users" PRIMARY KEY ("Id");


--
-- Name: Vehicles PK_Vehicles; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Vehicles"
    ADD CONSTRAINT "PK_Vehicles" PRIMARY KEY ("Id");


--
-- Name: Warehouses PK_Warehouses; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Warehouses"
    ADD CONSTRAINT "PK_Warehouses" PRIMARY KEY ("Id");


--
-- Name: IX_DeliveryAttempts_TenantId_DispatchStopId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_DeliveryAttempts_TenantId_DispatchStopId" ON public."DeliveryAttempts" USING btree ("TenantId", "DispatchStopId");


--
-- Name: IX_DispatchIncidents_TenantId_DispatchId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_DispatchIncidents_TenantId_DispatchId" ON public."DispatchIncidents" USING btree ("TenantId", "DispatchId");


--
-- Name: IX_DispatchSales_TenantId_DispatchId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_DispatchSales_TenantId_DispatchId" ON public."DispatchSales" USING btree ("TenantId", "DispatchId");


--
-- Name: IX_DispatchSales_TenantId_SaleId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_DispatchSales_TenantId_SaleId" ON public."DispatchSales" USING btree ("TenantId", "SaleId");


--
-- Name: IX_DispatchStops_TenantId_DispatchId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_DispatchStops_TenantId_DispatchId" ON public."DispatchStops" USING btree ("TenantId", "DispatchId");


--
-- Name: IX_GoodsReceiptLines_TenantId_GoodsReceiptId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_GoodsReceiptLines_TenantId_GoodsReceiptId" ON public."GoodsReceiptLines" USING btree ("TenantId", "GoodsReceiptId");


--
-- Name: IX_GoodsReceiptLines_TenantId_PurchaseOrderLineId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_GoodsReceiptLines_TenantId_PurchaseOrderLineId" ON public."GoodsReceiptLines" USING btree ("TenantId", "PurchaseOrderLineId");


--
-- Name: IX_GoodsReceipts_TenantId_PurchaseOrderId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_GoodsReceipts_TenantId_PurchaseOrderId" ON public."GoodsReceipts" USING btree ("TenantId", "PurchaseOrderId");


--
-- Name: IX_GoodsReceipts_TenantId_UserId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_GoodsReceipts_TenantId_UserId" ON public."GoodsReceipts" USING btree ("TenantId", "UserId");


--
-- Name: IX_InventoryMovements_TenantId_ProductId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_InventoryMovements_TenantId_ProductId" ON public."InventoryMovements" USING btree ("TenantId", "ProductId");


--
-- Name: IX_InventoryMovements_TenantId_UserId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_InventoryMovements_TenantId_UserId" ON public."InventoryMovements" USING btree ("TenantId", "UserId");


--
-- Name: IX_InventoryMovements_TenantId_WarehouseId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_InventoryMovements_TenantId_WarehouseId" ON public."InventoryMovements" USING btree ("TenantId", "WarehouseId");


--
-- Name: IX_Locations_TenantId_WarehouseId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Locations_TenantId_WarehouseId" ON public."Locations" USING btree ("TenantId", "WarehouseId");


--
-- Name: IX_Notifications_TenantId_EventKey; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Notifications_TenantId_EventKey" ON public."Notifications" USING btree ("TenantId", "EventKey");


--
-- Name: IX_Payments_TenantId_SaleId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Payments_TenantId_SaleId" ON public."Payments" USING btree ("TenantId", "SaleId");


--
-- Name: IX_ProductVariants_TenantId_ProductId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ProductVariants_TenantId_ProductId" ON public."ProductVariants" USING btree ("TenantId", "ProductId");


--
-- Name: IX_Products_TenantId_Sku; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Products_TenantId_Sku" ON public."Products" USING btree ("TenantId", "Sku");


--
-- Name: IX_PurchaseOrderLines_TenantId_ProductId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_PurchaseOrderLines_TenantId_ProductId" ON public."PurchaseOrderLines" USING btree ("TenantId", "ProductId");


--
-- Name: IX_PurchaseOrderLines_TenantId_PurchaseOrderId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_PurchaseOrderLines_TenantId_PurchaseOrderId" ON public."PurchaseOrderLines" USING btree ("TenantId", "PurchaseOrderId");


--
-- Name: IX_PurchaseOrders_TenantId_SupplierId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_PurchaseOrders_TenantId_SupplierId" ON public."PurchaseOrders" USING btree ("TenantId", "SupplierId");


--
-- Name: IX_PurchaseOrders_TenantId_WarehouseId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_PurchaseOrders_TenantId_WarehouseId" ON public."PurchaseOrders" USING btree ("TenantId", "WarehouseId");


--
-- Name: IX_ReturnLines_TenantId_ReturnId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ReturnLines_TenantId_ReturnId" ON public."ReturnLines" USING btree ("TenantId", "ReturnId");


--
-- Name: IX_ReturnLines_TenantId_SaleLineId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ReturnLines_TenantId_SaleLineId" ON public."ReturnLines" USING btree ("TenantId", "SaleLineId");


--
-- Name: IX_Returns_TenantId_SaleId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Returns_TenantId_SaleId" ON public."Returns" USING btree ("TenantId", "SaleId");


--
-- Name: IX_SaleLines_TenantId_ProductId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_SaleLines_TenantId_ProductId" ON public."SaleLines" USING btree ("TenantId", "ProductId");


--
-- Name: IX_SaleLines_TenantId_SaleId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_SaleLines_TenantId_SaleId" ON public."SaleLines" USING btree ("TenantId", "SaleId");


--
-- Name: IX_SalesReceipts_TenantId_Number; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_SalesReceipts_TenantId_Number" ON public."SalesReceipts" USING btree ("TenantId", "Number");


--
-- Name: IX_SalesReceipts_TenantId_SaleId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_SalesReceipts_TenantId_SaleId" ON public."SalesReceipts" USING btree ("TenantId", "SaleId");


--
-- Name: IX_Sales_TenantId_UserId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Sales_TenantId_UserId" ON public."Sales" USING btree ("TenantId", "UserId");


--
-- Name: IX_Sales_TenantId_WarehouseId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Sales_TenantId_WarehouseId" ON public."Sales" USING btree ("TenantId", "WarehouseId");


--
-- Name: IX_StockBalances_TenantId_ProductId_WarehouseId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_StockBalances_TenantId_ProductId_WarehouseId" ON public."StockBalances" USING btree ("TenantId", "ProductId", "WarehouseId");


--
-- Name: IX_StockBalances_TenantId_WarehouseId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_StockBalances_TenantId_WarehouseId" ON public."StockBalances" USING btree ("TenantId", "WarehouseId");


--
-- Name: IX_StockLots_TenantId_ProductId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_StockLots_TenantId_ProductId" ON public."StockLots" USING btree ("TenantId", "ProductId");


--
-- Name: IX_TenantSubscriptions_SubscriptionPlanId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_TenantSubscriptions_SubscriptionPlanId" ON public."TenantSubscriptions" USING btree ("SubscriptionPlanId");


--
-- Name: IX_Tenants_Ruc; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Tenants_Ruc" ON public."Tenants" USING btree ("Ruc");


--
-- Name: IX_UserRoles_TenantId_RoleId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserRoles_TenantId_RoleId" ON public."UserRoles" USING btree ("TenantId", "RoleId");


--
-- Name: IX_UserRoles_TenantId_UserId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserRoles_TenantId_UserId" ON public."UserRoles" USING btree ("TenantId", "UserId");


--
-- Name: IX_UserSessions_TenantId_UserId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserSessions_TenantId_UserId" ON public."UserSessions" USING btree ("TenantId", "UserId");


--
-- Name: IX_Users_Username; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Users_Username" ON public."Users" USING btree ("Username");


--
-- Name: Categories FK_Categorys_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Categories"
    ADD CONSTRAINT "FK_Categorys_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: Customers FK_Customers_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Customers"
    ADD CONSTRAINT "FK_Customers_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: DeliveryAttempts FK_DeliveryAttempts_DispatchStops_TenantId_DispatchStopId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DeliveryAttempts"
    ADD CONSTRAINT "FK_DeliveryAttempts_DispatchStops_TenantId_DispatchStopId" FOREIGN KEY ("TenantId", "DispatchStopId") REFERENCES public."DispatchStops"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: DeliveryAttempts FK_DeliveryAttempts_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DeliveryAttempts"
    ADD CONSTRAINT "FK_DeliveryAttempts_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: DispatchIncidents FK_DispatchIncidents_Dispatchs_TenantId_DispatchId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispatchIncidents"
    ADD CONSTRAINT "FK_DispatchIncidents_Dispatchs_TenantId_DispatchId" FOREIGN KEY ("TenantId", "DispatchId") REFERENCES public."Dispatches"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: DispatchIncidents FK_DispatchIncidents_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispatchIncidents"
    ADD CONSTRAINT "FK_DispatchIncidents_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: DispatchSales FK_DispatchSales_Dispatchs_TenantId_DispatchId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispatchSales"
    ADD CONSTRAINT "FK_DispatchSales_Dispatchs_TenantId_DispatchId" FOREIGN KEY ("TenantId", "DispatchId") REFERENCES public."Dispatches"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: DispatchSales FK_DispatchSales_Sales_TenantId_SaleId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispatchSales"
    ADD CONSTRAINT "FK_DispatchSales_Sales_TenantId_SaleId" FOREIGN KEY ("TenantId", "SaleId") REFERENCES public."Sales"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: DispatchSales FK_DispatchSales_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispatchSales"
    ADD CONSTRAINT "FK_DispatchSales_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: DispatchStops FK_DispatchStops_Dispatchs_TenantId_DispatchId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispatchStops"
    ADD CONSTRAINT "FK_DispatchStops_Dispatchs_TenantId_DispatchId" FOREIGN KEY ("TenantId", "DispatchId") REFERENCES public."Dispatches"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: DispatchStops FK_DispatchStops_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispatchStops"
    ADD CONSTRAINT "FK_DispatchStops_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: Dispatches FK_Dispatchs_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Dispatches"
    ADD CONSTRAINT "FK_Dispatchs_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: Drivers FK_Drivers_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Drivers"
    ADD CONSTRAINT "FK_Drivers_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: GoodsReceiptLines FK_GoodsReceiptLines_GoodsReceipts_TenantId_GoodsReceiptId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."GoodsReceiptLines"
    ADD CONSTRAINT "FK_GoodsReceiptLines_GoodsReceipts_TenantId_GoodsReceiptId" FOREIGN KEY ("TenantId", "GoodsReceiptId") REFERENCES public."GoodsReceipts"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: GoodsReceiptLines FK_GoodsReceiptLines_PurchaseOrderLines_TenantId_PurchaseOrder~; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."GoodsReceiptLines"
    ADD CONSTRAINT "FK_GoodsReceiptLines_PurchaseOrderLines_TenantId_PurchaseOrder~" FOREIGN KEY ("TenantId", "PurchaseOrderLineId") REFERENCES public."PurchaseOrderLines"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: GoodsReceiptLines FK_GoodsReceiptLines_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."GoodsReceiptLines"
    ADD CONSTRAINT "FK_GoodsReceiptLines_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: GoodsReceipts FK_GoodsReceipts_PurchaseOrders_TenantId_PurchaseOrderId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."GoodsReceipts"
    ADD CONSTRAINT "FK_GoodsReceipts_PurchaseOrders_TenantId_PurchaseOrderId" FOREIGN KEY ("TenantId", "PurchaseOrderId") REFERENCES public."PurchaseOrders"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: GoodsReceipts FK_GoodsReceipts_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."GoodsReceipts"
    ADD CONSTRAINT "FK_GoodsReceipts_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: GoodsReceipts FK_GoodsReceipts_Users_TenantId_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."GoodsReceipts"
    ADD CONSTRAINT "FK_GoodsReceipts_Users_TenantId_UserId" FOREIGN KEY ("TenantId", "UserId") REFERENCES public."Users"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: InventoryMovements FK_InventoryMovements_Products_TenantId_ProductId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InventoryMovements"
    ADD CONSTRAINT "FK_InventoryMovements_Products_TenantId_ProductId" FOREIGN KEY ("TenantId", "ProductId") REFERENCES public."Products"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: InventoryMovements FK_InventoryMovements_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InventoryMovements"
    ADD CONSTRAINT "FK_InventoryMovements_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: InventoryMovements FK_InventoryMovements_Users_TenantId_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InventoryMovements"
    ADD CONSTRAINT "FK_InventoryMovements_Users_TenantId_UserId" FOREIGN KEY ("TenantId", "UserId") REFERENCES public."Users"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: InventoryMovements FK_InventoryMovements_Warehouses_TenantId_WarehouseId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InventoryMovements"
    ADD CONSTRAINT "FK_InventoryMovements_Warehouses_TenantId_WarehouseId" FOREIGN KEY ("TenantId", "WarehouseId") REFERENCES public."Warehouses"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: Locations FK_Locations_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Locations"
    ADD CONSTRAINT "FK_Locations_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: Locations FK_Locations_Warehouses_TenantId_WarehouseId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Locations"
    ADD CONSTRAINT "FK_Locations_Warehouses_TenantId_WarehouseId" FOREIGN KEY ("TenantId", "WarehouseId") REFERENCES public."Warehouses"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: Notifications FK_Notifications_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Notifications"
    ADD CONSTRAINT "FK_Notifications_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: Payments FK_Payments_Sales_TenantId_SaleId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Payments"
    ADD CONSTRAINT "FK_Payments_Sales_TenantId_SaleId" FOREIGN KEY ("TenantId", "SaleId") REFERENCES public."Sales"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: Payments FK_Payments_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Payments"
    ADD CONSTRAINT "FK_Payments_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: ProductVariants FK_ProductVariants_Products_TenantId_ProductId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ProductVariants"
    ADD CONSTRAINT "FK_ProductVariants_Products_TenantId_ProductId" FOREIGN KEY ("TenantId", "ProductId") REFERENCES public."Products"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: ProductVariants FK_ProductVariants_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ProductVariants"
    ADD CONSTRAINT "FK_ProductVariants_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: Products FK_Products_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Products"
    ADD CONSTRAINT "FK_Products_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: PurchaseOrderLines FK_PurchaseOrderLines_Products_TenantId_ProductId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PurchaseOrderLines"
    ADD CONSTRAINT "FK_PurchaseOrderLines_Products_TenantId_ProductId" FOREIGN KEY ("TenantId", "ProductId") REFERENCES public."Products"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: PurchaseOrderLines FK_PurchaseOrderLines_PurchaseOrders_TenantId_PurchaseOrderId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PurchaseOrderLines"
    ADD CONSTRAINT "FK_PurchaseOrderLines_PurchaseOrders_TenantId_PurchaseOrderId" FOREIGN KEY ("TenantId", "PurchaseOrderId") REFERENCES public."PurchaseOrders"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: PurchaseOrderLines FK_PurchaseOrderLines_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PurchaseOrderLines"
    ADD CONSTRAINT "FK_PurchaseOrderLines_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: PurchaseOrders FK_PurchaseOrders_Suppliers_TenantId_SupplierId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PurchaseOrders"
    ADD CONSTRAINT "FK_PurchaseOrders_Suppliers_TenantId_SupplierId" FOREIGN KEY ("TenantId", "SupplierId") REFERENCES public."Suppliers"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: PurchaseOrders FK_PurchaseOrders_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PurchaseOrders"
    ADD CONSTRAINT "FK_PurchaseOrders_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: PurchaseOrders FK_PurchaseOrders_Warehouses_TenantId_WarehouseId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PurchaseOrders"
    ADD CONSTRAINT "FK_PurchaseOrders_Warehouses_TenantId_WarehouseId" FOREIGN KEY ("TenantId", "WarehouseId") REFERENCES public."Warehouses"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: ReturnLines FK_ReturnLines_Returns_TenantId_ReturnId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ReturnLines"
    ADD CONSTRAINT "FK_ReturnLines_Returns_TenantId_ReturnId" FOREIGN KEY ("TenantId", "ReturnId") REFERENCES public."Returns"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: ReturnLines FK_ReturnLines_SaleLines_TenantId_SaleLineId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ReturnLines"
    ADD CONSTRAINT "FK_ReturnLines_SaleLines_TenantId_SaleLineId" FOREIGN KEY ("TenantId", "SaleLineId") REFERENCES public."SaleLines"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: ReturnLines FK_ReturnLines_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ReturnLines"
    ADD CONSTRAINT "FK_ReturnLines_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: Returns FK_Returns_Sales_TenantId_SaleId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Returns"
    ADD CONSTRAINT "FK_Returns_Sales_TenantId_SaleId" FOREIGN KEY ("TenantId", "SaleId") REFERENCES public."Sales"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: Returns FK_Returns_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Returns"
    ADD CONSTRAINT "FK_Returns_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: Roles FK_Roles_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Roles"
    ADD CONSTRAINT "FK_Roles_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: SaleLines FK_SaleLines_Products_TenantId_ProductId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SaleLines"
    ADD CONSTRAINT "FK_SaleLines_Products_TenantId_ProductId" FOREIGN KEY ("TenantId", "ProductId") REFERENCES public."Products"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: SaleLines FK_SaleLines_Sales_TenantId_SaleId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SaleLines"
    ADD CONSTRAINT "FK_SaleLines_Sales_TenantId_SaleId" FOREIGN KEY ("TenantId", "SaleId") REFERENCES public."Sales"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: SaleLines FK_SaleLines_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SaleLines"
    ADD CONSTRAINT "FK_SaleLines_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: SalesReceipts FK_SalesReceipts_Sales_TenantId_SaleId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SalesReceipts"
    ADD CONSTRAINT "FK_SalesReceipts_Sales_TenantId_SaleId" FOREIGN KEY ("TenantId", "SaleId") REFERENCES public."Sales"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: SalesReceipts FK_SalesReceipts_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SalesReceipts"
    ADD CONSTRAINT "FK_SalesReceipts_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: Sales FK_Sales_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Sales"
    ADD CONSTRAINT "FK_Sales_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: Sales FK_Sales_Users_TenantId_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Sales"
    ADD CONSTRAINT "FK_Sales_Users_TenantId_UserId" FOREIGN KEY ("TenantId", "UserId") REFERENCES public."Users"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: Sales FK_Sales_Warehouses_TenantId_WarehouseId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Sales"
    ADD CONSTRAINT "FK_Sales_Warehouses_TenantId_WarehouseId" FOREIGN KEY ("TenantId", "WarehouseId") REFERENCES public."Warehouses"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: StockBalances FK_StockBalances_Products_TenantId_ProductId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockBalances"
    ADD CONSTRAINT "FK_StockBalances_Products_TenantId_ProductId" FOREIGN KEY ("TenantId", "ProductId") REFERENCES public."Products"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: StockBalances FK_StockBalances_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockBalances"
    ADD CONSTRAINT "FK_StockBalances_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: StockBalances FK_StockBalances_Warehouses_TenantId_WarehouseId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockBalances"
    ADD CONSTRAINT "FK_StockBalances_Warehouses_TenantId_WarehouseId" FOREIGN KEY ("TenantId", "WarehouseId") REFERENCES public."Warehouses"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: StockLots FK_StockLots_Products_TenantId_ProductId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockLots"
    ADD CONSTRAINT "FK_StockLots_Products_TenantId_ProductId" FOREIGN KEY ("TenantId", "ProductId") REFERENCES public."Products"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: StockLots FK_StockLots_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockLots"
    ADD CONSTRAINT "FK_StockLots_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: Suppliers FK_Suppliers_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Suppliers"
    ADD CONSTRAINT "FK_Suppliers_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: TenantSubscriptions FK_TenantSubscriptions_SubscriptionPlans_SubscriptionPlanId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."TenantSubscriptions"
    ADD CONSTRAINT "FK_TenantSubscriptions_SubscriptionPlans_SubscriptionPlanId" FOREIGN KEY ("SubscriptionPlanId") REFERENCES public."SubscriptionPlans"("Id") ON DELETE RESTRICT;


--
-- Name: TenantSubscriptions FK_TenantSubscriptions_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."TenantSubscriptions"
    ADD CONSTRAINT "FK_TenantSubscriptions_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: UserRoles FK_UserRoles_Roles_TenantId_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserRoles"
    ADD CONSTRAINT "FK_UserRoles_Roles_TenantId_RoleId" FOREIGN KEY ("TenantId", "RoleId") REFERENCES public."Roles"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: UserRoles FK_UserRoles_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserRoles"
    ADD CONSTRAINT "FK_UserRoles_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: UserRoles FK_UserRoles_Users_TenantId_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserRoles"
    ADD CONSTRAINT "FK_UserRoles_Users_TenantId_UserId" FOREIGN KEY ("TenantId", "UserId") REFERENCES public."Users"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: UserSessions FK_UserSessions_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserSessions"
    ADD CONSTRAINT "FK_UserSessions_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: UserSessions FK_UserSessions_Users_TenantId_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserSessions"
    ADD CONSTRAINT "FK_UserSessions_Users_TenantId_UserId" FOREIGN KEY ("TenantId", "UserId") REFERENCES public."Users"("TenantId", "Id") ON DELETE RESTRICT;


--
-- Name: Users FK_Users_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "FK_Users_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: Vehicles FK_Vehicles_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Vehicles"
    ADD CONSTRAINT "FK_Vehicles_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- Name: Warehouses FK_Warehouses_Tenants_TenantId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Warehouses"
    ADD CONSTRAINT "FK_Warehouses_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES public."Tenants"("Id") ON DELETE RESTRICT;


--
-- PostgreSQL database dump complete
--

\unrestrict okFGRH4KHbV37eif6P7ldcre84Dp5FLAakwxOc8ZD7qygdo0TuNHsFjuPsRwoIZ

