# Product and Order MCP Demo — Implementation and Resume Plan

Created: 2026-10-16
Status: Planning complete; implementation has not started.

## Learning mode: execute one lesson at a time

The goal is both a working demo and an understanding of MCP from a software
architect's perspective. The lesson sequence below is the primary execution
order; the implementation checklist later is a milestone summary, not permission
to implement everything in one turn.

When the user requests a lesson, complete ONLY that lesson and its verification,
then stop for review. Do not automatically begin the next lesson. Explain the
architecture decision before coding, keep changes small, and finish by stating
what changed, what was verified, and one question the learner can answer to check
understanding. Update the lesson checkbox and handoff notes. If a prerequisite
is missing, explain it and complete only prerequisite work needed by that lesson.
These deliberate lesson boundaries are the user's requested learning workflow.

For each lesson use this format:

1. Explain the concept and the decision in this repository, including a relevant
   tradeoff. Distinguish MCP protocol behavior from application business rules.
2. Identify files and describe the smallest implementation for this lesson.
3. Implement when requested; explain meaningful code without narrating every line.
4. Run the lesson's check and show the expected versus actual outcome. Record
   unavailable dependencies honestly; do not mark unverified work as verified.
5. Update this document and stop with a short understanding check.

Reusable prompt:

> Read MCP-DEMO-PLAN.md. Execute lesson N only in learning mode. Explain the
> architectural choices, implement the small change, verify it, update the plan,
> and stop before the next lesson.

For discussion without code:

> Explain lesson N from MCP-DEMO-PLAN.md without changing application code.

## Ordered lessons

### Lesson 1: Map the system and establish a baseline

- [ ] Completed
- Learn: responsibilities of the AI host, MCP client, MCP server, REST API,
  application service, and database. MCP provides tool discovery/invocation;
  it does not supply your business workflow or automatically grant write access.
- Do: inspect local instructions, current routes, dependency lifetimes, Nuxt
  page structure, and database startup behavior. Draw the current and proposed
  request paths. Build the API without starting it or applying migrations.
- Check: record baseline build results and existing failures; locate the demo
  database configuration without exposing credentials. No schema/data changes.
- Discuss: why is the approval boundary separate from shared CRUD validation?

### Lesson 2: Define the MCP contract and connect a read-only server

- [ ] Completed
- Learn: initialization/capability negotiation, tools/list, tools/call, JSON
  schemas, tool descriptions, and Streamable HTTP versus stdio. Explain tools,
  resources, and prompts, and why this demo starts with tools.
- Do: select and record an SDK version compatible with net9.0, add `/mcp`, and
  expose only list_products and get_product. Inspect how the selected SDK
  resolves scoped services; do not capture DbContext in a singleton.
- Check: use an MCP client/Inspector to initialize, discover schemas, list
  products, and handle a missing ID. Verify only read tools are available.
- Discuss: why is a REST route not automatically an MCP tool?

### Lesson 3: Complete read tools and design write contracts

- [ ] Completed
- Learn: typed contracts, model-facing descriptions, structured results and
  errors, domain boundaries, and live reads versus manually indexed RAG data.
- Do: add order reads and shared insights; specify write payloads and pending
  result schemas. Define IDs, validation, replacement semantics, and status
  tracking. Keep write tools unregistered until queue persistence is ready.
- Check: discover and call each read tool; document example pending-write
  responses and explain why pending creates cannot supply an actual entity ID.
- Discuss: which rules belong in tool metadata and which need server enforcement?

### Lesson 4: Build shared product mutation behavior

- [ ] Completed
- Learn: application service reuse, adapter boundaries, validation, and
  referential integrity versus convenient cascading deletes.
- Do: implement product update/delete, DTOs, REST routes, and validation in
  shared services. Keep both IInMemoryStore implementations compiling. Add the
  restricted product-deletion FK migration; inspect it before applying it to
  a verified disposable database.
- Check: product create/update/delete via REST, bad inputs, missing IDs, and
  rejection of deletion when orders exist. No MCP writes yet.
- Discuss: why should approval and REST reuse the same mutation logic?

### Lesson 5: Implement order and inventory invariants

- [ ] Completed
- Learn: transaction boundaries, historical prices, concurrency, and business
  invariants across multiple rows.
- Do: implement positive quantity validation, stock deduction/restoration,
  quantity updates, individual deletion, compatible clearing, and REST routes.
  Choose concurrency protection explicitly; preserve captured UnitPrice.
- Check: exercise the stock/price rules below, failed-write rollback, and
  concurrent stock claims with SQL Server. Use fresh demo data because legacy
  orders did not deduct inventory.
- Discuss: why is a read-then-write stock check alone insufficient?

### Lesson 6: Persist MCP change proposals

- [ ] Completed
- Learn: a proposal is a durable command awaiting review, not an executed
  mutation. Compare durable tables with connection-scoped SQL temporary tables.
- Do: add PendingChanges and its migration, an allowlisted operation/payload
  model, Pending status, baseline snapshot/version data, and submission/status
  services. Implement no approval execution yet.
- Check: submit a proposal through a focused service test; business records
  remain unchanged and the pending request survives a process restart.
- Discuss: why store structured intent instead of arbitrary executable SQL?

### Lesson 7: Expose queued writes through MCP

- [ ] Completed
- Learn: semantic tool naming, discovery, failure reporting, and explicit
  pending-versus-applied outcomes. Annotations cannot enforce approval.
- Do: register all six write tools as proposal submitters and add read-only
  get_change_request. Return request IDs and Pending status. Do not expose
  approval, raw SQL, or immediate-write bypass tools.
- Check: submit each operation through MCP; inspect persisted payloads and
  verify products, orders, and inventory remain unchanged.
- Discuss: how does an assistant know when a proposed create actually exists?

### Lesson 8: Implement the approval state machine and API

- [ ] Completed
- Learn: Pending -> Applied/Rejected/Failed, stale-data detection, idempotency,
  and transactional consistency of command execution and status recording.
- Do: add list/detail/approve/reject routes and the approval executor. Call
  shared CRUD services in-process. Use the transaction and concurrency rules
  below; reject stale requests and revalidate stock/prices at approval time.
- Check: approve once, reject, retry approve, race approve/reject, change a target
  through REST before approval, and force an execution failure. Verify no partial
  mutations and no duplicate effects. Direct REST writes still execute immediately.
- Discuss: what can go wrong if mutation commits before approval status does?

### Lesson 9: Build the human review page

- [ ] Completed
- Learn: the review UI as a decision surface, current versus proposed values,
  and separating human review from model tool access.
- Do: add Nuxt `/approvals` using existing project conventions, request details,
  comparisons, Approve/Reject actions, status, result IDs, and readable errors.
  Handle loading and duplicate clicks; backend checks remain authoritative.
- Check: review and apply/reject real MCP proposals, refresh the page, and verify
  results. Show stale-state failures and confirm ordinary UI CRUD remains immediate.
- Discuss: why does disabling a button not prevent duplicate execution?

### Lesson 10: Run the full demo and review the architecture

- [ ] Completed
- Learn: integration evidence, deployment boundaries, observability, and the
  difference between this local demo and a remotely deployed service.
- Do: run the acceptance sequence below, document client setup and run commands,
  and review operation/request correlation, remaining security boundaries, index
  staleness, and scaling limitations. Document follow-ups without adding scope.
- Check: API build, targeted tests, MCP discovery/calls, approval scenarios,
  persistence after restart, and UI regression checks. Record any unverified items.
- Discuss: how would separate MCP/API processes change transactions, identity,
  idempotency, and failure recovery?

## Goal and authorization

Build a demo MCP server that lets an MCP-capable AI client list, create, update,
and delete products and orders stored by this application in SQL Server.
Confirmed scope: approval is required ONLY for MCP-originated writes. Existing
UI and direct REST CRUD requests continue to execute immediately. MCP reads are
immediate; MCP writes create pending requests for a human to approve or reject.
The user's current request is to create and save this plan. Do not interpret this
document alone as authorization to start implementation; start when the user asks
to implement or resume it. During implementation, keep the checklist and handoff
notes below current so another Codex session can continue.

## Resume instructions for Codex

1. Read this document and any applicable AGENTS.md instructions.
2. Run `git status --short` and inspect existing changes before editing.
3. Read the files listed below and compare actual code with this plan. The
   checklist is a guide; verify completed work before relying on it.
4. Execute only the requested lesson; if asked to continue without a lesson
   number, choose the first unfinished lesson and stop after it. Preserve
   unrelated work. Do not read or print secrets from .env or connection strings.
5. Check the official MCP C# SDK documentation for the selected package version;
   APIs and supported frameworks can change. Record the chosen version here.
6. After each meaningful milestone, record changes, checks, blockers, and the
   next action in the handoff section. Do not claim checks passed without running them.

Suggested prompt for a future session:

> Read MCP-DEMO-PLAN.md, inspect the repository and current changes, and continue
> with the first unfinished lesson only, in learning mode. Explain, implement,
> verify, update the plan, and stop before the next lesson.

## Repository findings

- `CustomerOrders.Api/Program.cs`: ASP.NET Core minimal API; product/order reads,
  product creation, order creation, bulk order clearing, order insights, and
  separate AI/RAG/vector/SQL Copilot endpoints.
- `CustomerOrders.Api/CustomerOrders.Api.csproj`: targets net9.0; EF Core SQL
  Server 9.0.0; currently no MCP package reference.
- `CustomerOrders.Api/Services/InMemoryStore.cs`: declares IInMemoryStore AND
  a concrete InMemoryStore. Both implementations must compile if the interface changes.
- `CustomerOrders.Api/Services/ProductOrderDbService.cs`: actual scoped
  IInMemoryStore implementation registered in Program.cs; persists via AppDbContext.
- `CustomerOrders.Api/Models/ProductDto.cs`: ProductDto and CreateProductRequest.
- `CustomerOrders.Api/Models/OrderDto.cs`: OrderDto and CreateOrderRequest.
- `CustomerOrders.Api/Data/AppDbContext.cs`: product/order mapping, field length
  limits, decimal precision, and cascading deletion from products to orders.
- `CustomerOrders.Api/Data/Entities/Product.cs` and `Order.cs`: one product per
  order; UnitPrice is captured on the order.
- Startup automatically calls Database.Migrate(); running the application can
  change the configured database. Verify a disposable demo database before running.
- No implementation changes existed when this plan was created.

## Proposed architecture

Host a Streamable HTTP MCP endpoint at `/mcp` in the existing API process.

    MCP reads  -> ProductTools / OrderTools -> shared application service -> SQL Server
    MCP writes -> ProductTools / OrderTools -> PendingChanges table
    Approval page -> approval API -> approved operation -> shared CRUD service -> SQL Server
    Existing Nuxt UI -> existing REST endpoints -> shared CRUD service -> SQL Server

Keep existing CRUD endpoints as the immediate-write path. The approval executor
reuses their application service methods in the same process. This preserves one
implementation of mutations without a loopback HTTP call. Literal calls to REST
endpoints are an alternative if specifically requested; they require additional
idempotency and failure handling across the HTTP boundary.

Use the official ModelContextProtocol.AspNetCore package, with a release compatible
with net9.0. Register tools using the selected SDK's supported AddMcpServer,
WithHttpTransport, WithTools, and MapMcp APIs. Tool files belong under
`CustomerOrders.Api/Mcp/`. Keep business rules in the shared service and respect
the scoped lifetime of AppDbContext; do not capture it in singleton tools.

For the first iteration, retain the IInMemoryStore name to limit refactoring.
No new LLM calls or API keys are required by the CRUD tools themselves: the MCP
client handles language interpretation and invokes the server's typed tools.
Do not route CRUD through generated SQL or the SQL Copilot execute endpoint.

Official references to verify during implementation:

- https://github.com/modelcontextprotocol/csharp-sdk
- https://github.com/modelcontextprotocol/csharp-sdk/blob/main/docs/concepts/getting-started.md
- https://github.com/modelcontextprotocol/csharp-sdk/blob/main/docs/concepts/tools/tools.md

## Proposed tool contract

| Tool | Inputs | Behavior |
| --- | --- | --- |
| list_products | none initially | Return products and IDs |
| get_product | productId | Return one product |
| create_product | name, description, price, stock | Queue creation for approval |
| update_product | productId, name, description, price, stock | Queue field replacement for approval |
| delete_product | productId | Queue deletion for approval |
| list_orders | none initially | Return orders and IDs |
| get_order | orderId | Return one order |
| create_order | productId, quantity | Queue creation for approval |
| update_order | orderId, quantity | Queue quantity change for approval |
| delete_order | orderId | Queue individual deletion for approval |
| get_order_insights | none | Return existing totals and top products |

UpdateOrder changes quantity only. Changing a product on an existing order is
outside the initial scope. Use returned IDs in subsequent tool calls; the client
must resolve ambiguous product names before a write. Keep tool descriptions clear
about replacement semantics, inventory changes, and destructive operations.
Return useful typed results and SDK-supported tool errors for validation,
missing IDs, insufficient stock, and deletion conflicts. REST equivalents should
return 400, 404, and 409 as appropriate without leaking internal exceptions.

Every MCP write returns a request ID and Pending status, explicitly stating that
business data has not changed. A queued create returns no new product/order ID;
dependent requests must wait for approval and use the actual result ID. Optionally
expose a read-only get_change_request tool for tracking status and results. Do
not expose approve/reject tools to the MCP client or an alternative immediate
write tool that bypasses review.

## MCP-only approval workflow

Use a persistent PendingChanges table rather than a SQL #temp table, so requests
survive connections and restarts. Store structured operations, not executable SQL.
Suggested fields: Id, EntityType, Operation, EntityId (nullable for creates),
PayloadJson, OriginalSnapshotJson or target version, Status, CreatedAtUtc,
ReviewedAtUtc, AppliedAtUtc, ResultEntityId, and ErrorMessage. Add concurrency
protection for competing approvals/rejections.

Add a Nuxt `/approvals` page listing requests with current and proposed values,
status, execution results, and Approve/Reject buttons for pending items. Approval
routes may use GET `/api/mcp/changes`, GET `/api/mcp/changes/{id}`, POST
`/api/mcp/changes/{id}/approve`, and POST `/api/mcp/changes/{id}/reject`.

- Submission validates payload shape and stores the request without changing
  products, orders, or stock. No generated SQL is executed.
- Approve revalidates against current data and executes the fixed, allowlisted
  operation through the shared CRUD service. Detect intervening target changes
  using a version/snapshot and require a new proposal instead of silently
  overwriting changes made through the immediate REST path.
- Apply the mutation and mark the request Applied in one database transaction.
  Coordinate existing service SaveChanges calls with that transaction. Duplicate
  approvals and concurrent approve/reject requests must produce at most one effect.
- Reject transitions Pending to Rejected and never changes business data.
- On execution failure, roll back business changes and record Failed with a
  useful error separately. Failed/Rejected/Applied requests cannot be executed
  again; submit a new request after correcting the cause.
- Order price/stock rules below take effect at approval time. Show live values
  in the review page and reject stale proposals affecting those values.
- Existing REST endpoints do not create approval requests. Keep approval
  orchestration separate from shared CRUD methods to avoid gating ordinary UI use.
- This is a local demo review boundary. If authentication is added later,
  reviewer authorization must be enforced separately from MCP access.

## Proposed business rules

These are implementation defaults proposed for the demo, not existing behavior.

- Product name: trimmed, required, maximum 100 characters. Description: non-null,
  maximum 500 characters. Price >= 0 and stock >= 0; use decimal for money and
  explicitly validate the stored decimal(18,2) range/scale.
- Order quantity must be positive. Verify product existence and available stock.
- Create order: subtract quantity from stock and capture current product price.
- Update order: preserve UnitPrice and CreatedAtUtc; subtract the quantity delta
  from stock (a reduction returns stock). Reject increases beyond availability.
- Delete order: restore stock and remove the order together.
- Product price changes do not reprice existing orders. Product names currently
  come from the related product, so renaming a product changes displayed order names.
- Product stock updates set current available stock; describe this explicitly.
- Reject deletion of products with orders. Change the FK to Restrict/NoAction
  using an EF migration so the database also enforces this policy.
- Keep each order mutation and inventory change atomic. Use an appropriate
  transaction/concurrency approach so concurrent requests cannot oversell stock
  or restore the same order twice. Verify with SQL Server integration coverage.
- Existing orders were created without stock deductions. Use a fresh demo
  database for inventory tests; do not silently reinterpret or reset existing data.
- Keep bulk ClearOrders outside the MCP tool set. Since it remains a REST action,
  align its inventory behavior with the new rules and document the compatibility
  change; do not leave it inconsistent with individual order deletion.

## Implementation checklist

Milestone summary only: follow the ordered lessons above for execution. Lessons
intentionally introduce read-only MCP early, before mutation and approval work.

- [x] Inspect Program.cs, persistence implementation, models, and mappings.
- [x] Save architecture, proposed rules, verification, and resume instructions.
- [ ] 1. Establish baseline: inspect local instructions, build API, identify demo
      database/client setup, and record SDK/package choice. Do not expose secrets.
- [ ] 2. Add update DTOs and shared service methods for product/order updates,
      individual deletion, and insights. Keep both store implementations compiling
      and behavior consistent; avoid duplicating rules in REST and MCP adapters.
- [ ] 3. Implement validation, stock handling, atomic writes, deletion conflict
      behavior, and compatible bulk clearing. Add the FK restriction migration
      and any concurrency schema changes required by the chosen approach.
- [ ] 4. Add REST PUT `/api/products/{id}`, DELETE `/api/products/{id}`,
      PUT `/api/orders/{id}`, and DELETE `/api/orders/{id}`. Retain existing
      response shapes/routes where possible; map domain failures consistently.
- [ ] 5. Add PendingChanges entity/migration, request submission, state transitions,
      stale-data checks, and atomic approval execution. Add approval API routes.
- [ ] 6. Add the Nuxt `/approvals` page with comparison, Approve/Reject actions,
      pending/completed states, and clear errors. Inspect existing UI conventions.
- [ ] 7. Install compatible MCP package; add ProductTools.cs and OrderTools.cs;
      register the HTTP transport and `/mcp`. Reads execute immediately; all writes
      enqueue requests. Check DI lifetimes, errors, and absence of bypass tools.
- [ ] 8. Run targeted tests and MCP integration checks described below; fix failures.
- [ ] 9. Document actual run commands, endpoint URL, tested client configuration,
      demo prompts, and expected results. Record any remaining limitations here.

## Verification and acceptance

- Build: `dotnet build CustomerOrders.Api/CustomerOrders.Api.csproj`.
- Add focused tests for validation, missing IDs, product deletion conflicts,
  historical pricing, stock changes, and rollback on failed writes. Cover create,
  increase/decrease quantity, delete, and clear with invariant assertions.
- Verify FK enforcement, atomic stock/order changes, and competing requests with
  a disposable SQL Server database. An in-memory provider cannot prove these.
- Connect an MCP client or official MCP Inspector using Streamable HTTP; verify
  initialization, tools/list schemas, all CRUD calls, and readable failure responses.
- Confirm REST and MCP see the same persisted records, including after restart.
- Confirm the existing Nuxt product/order flows and insights still work.
- Verify MCP submissions leave business data unchanged; approval applies once;
  rejection has no effect; requests persist after restart. Test stale proposals,
  rollback on failure, duplicate approval, and concurrent approve/reject calls.
- Verify direct REST/UI writes remain immediate without pending requests. Verify
  there is no MCP tool that executes writes or approves requests directly.
- Do not claim an end-to-end pass if SQL Server or an MCP client is unavailable;
  record exactly what could and could not be checked.

Demo sequence (use IDs returned by tools):

For every successful MCP write below, first verify Pending status and unchanged
business data, approve it on `/approvals`, then verify the result. Wait for an
approved create's result ID before proposing dependent changes. Also submit a
separate change and reject it to demonstrate that data remains unchanged.

1. Create Wireless Keyboard, description Demo keyboard, price 49.99, stock 20.
2. Create an order for 2: available stock should become 18.
3. Change quantity to 3: stock should become 17; order total should be 149.97.
4. Change product price to 44.99 while retaining current stock: order UnitPrice
   should remain 49.99.
5. Show order and insights; attempt product deletion and receive a conflict.
6. Delete order: stock should return to 20; then delete product successfully.
7. Try zero quantity, insufficient stock, and a nonexistent ID; verify useful errors
   and unchanged database state after each failure. Some failures may be detected
   during submission; current-state business conflicts must also be checked on approval.

## Scope and follow-up work

Initial scope: local demo, existing API host, typed tools, SQL persistence, and
correct CRUD/inventory behavior. Use loopback hosting for the demo; do not publish
an unauthenticated write endpoint. Authentication and remote hosting are separate
follow-up work if requested. Existing AI/RAG/vector features remain separate;
their manually built indexes can become stale after writes. Use live CRUD reads
for this demo, and document that index rebuilding/synchronization is not included.
Tool annotations describe behavior but do not enforce user approval or authorization.

## Handoff notes

- Last completed action: updated plan for confirmed MCP-only approval workflow;
  existing UI/REST writes remain immediate. No application code changed.
- Next action: when requested, execute Lesson 1 only and stop for learning review.
- Learning progress: no lessons executed yet; architecture and plan documented.
- Chosen MCP package version: not selected yet.
- Build/tests: not run; planning-only task.
- MCP client and demo database: not selected or verified yet.
- Blockers: none for planning. Implementation requires package restore and a
  configured disposable SQL Server database for full verification.
