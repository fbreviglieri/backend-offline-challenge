# Part 4 — Azure API Management

Scenario: a new .NET backend API is being developed for consumption by external clients through
Azure API Management (APIM).

## 1. Main steps to publish and secure the API in APIM

1. **Design and document the API first.** Build/annotate the .NET API with OpenAPI (Swagger)
   support (Swashbuckle / `Microsoft.AspNetCore.OpenApi`) so it produces an accurate OpenAPI
   spec — this becomes the source of truth for the APIM import.
2. **Import the API into APIM** from the OpenAPI definition (or via an Azure Functions/App
   Service/Container App "one-click" import). This creates the API surface, operations, and
   request/response schemas in APIM.
3. **Secure the hop between APIM and the backend** (the part clients never see): restrict the
   backend so it's only reachable through APIM — e.g. backend requires a mutual-TLS client
   certificate or a managed-identity-issued token from APIM, and/or the backend sits behind a
   private endpoint/VNet integration with APIM in internal mode, so it can't be hit directly
   over the public internet.
4. **Secure the hop between clients and APIM** — decide the auth model per client type:
   - Machine-to-machine/partner clients → **subscription keys** (coarse access + usage
     tracking) and/or **OAuth2 client-credentials** validated with the `validate-jwt` policy
     against Azure AD (Entra ID).
   - End-user-facing clients → OAuth2/OpenID Connect via Azure AD / Azure AD B2C, again
     enforced with `validate-jwt`, so APIM rejects unauthenticated/invalid-token calls before
     they ever reach the backend.
5. **Organize the API surface**: group operations into a **Product** (e.g. "Partner API v1"),
   set its visibility/subscription-required flag, and use **API versions/revisions** so a
   breaking change ships as a new version while existing clients keep working on the old one.
   Test a revision privately before making it current.
6. **Configure a custom domain + TLS certificate** for the APIM gateway so clients hit a
   branded, certificate-backed hostname rather than the default `*.azure-api.net` one.
7. **Automate the rollout.** Define the API, products, and policies as code (Bicep/ARM/
   Terraform, or the APIM DevOps Resource Kit) and deploy through a CI/CD pipeline
   (GitHub Actions/Azure DevOps) so dev → test → prod promotion is repeatable and reviewable,
   rather than clicking through the portal.
8. **Store secrets outside the policy XML.** Anything sensitive (backend API keys, certificate
   thumbprints, client secrets) goes into APIM **Named Values** backed by **Key Vault**, never
   hardcoded into a policy.
9. **Validate through the developer portal** before general availability — publish the API to
   the APIM developer portal so consumers can self-serve API docs, get a subscription key, and
   try calls, and use that same portal to sanity-check the contract end-to-end.

## 2. Policies and features for access control and monitoring

**Access control**
- `validate-jwt` — verify the bearer token's issuer/audience/signature/expiry (Azure AD or any
  OIDC provider) before a request reaches the backend; reject with 401 otherwise.
- **Subscription keys + Products** — the simplest access-tiering mechanism; different products
  (e.g. "Free", "Partner") can expose different operations or rate limits to different
  consumers.
- `ip-filter` — allow/deny by caller IP range, useful for partner-to-partner integrations with
  known egress IPs.
- `cors` — restrict which origins may call the API from a browser.
- `check-header` — enforce presence/format of custom headers (e.g. a correlation ID or a
  partner-specific header) before forwarding the request.
- **Managed identity** (`authentication-managed-identity` policy) — lets APIM authenticate to
  the backend (or to other Azure resources) without embedding any credential at all.

**Rate limiting / resilience**
- `rate-limit` / `rate-limit-by-key` — short-window throttling (e.g. per subscription or per
  client IP) to protect the backend from bursts.
- `quota` / `quota-by-key` — longer-window usage caps (e.g. "10,000 calls/month") tied to a
  subscription tier.
- `cache-lookup` / `cache-store` — cache idempotent GET responses at the gateway to cut backend
  load and latency for hot, rarely-changing data.

**Monitoring and usage visibility**
- **Application Insights integration** — attach APIM to an App Insights resource to get
  distributed tracing across gateway → backend, including latency and failure correlation.
- **Diagnostic settings / Azure Monitor** — stream gateway logs and metrics to Log Analytics for
  alerting (e.g. alert on elevated 5xx rate or p95 latency).
- **Built-in APIM Analytics** dashboard — per-product/per-subscription call volume, latency, and
  error breakdowns, useful for both operational monitoring and partner usage reporting/billing.
- `log-to-eventhub` policy — for teams that want to pipe every request/response (or a sampled
  subset) into their own downstream analytics pipeline instead of relying solely on the built-in
  dashboards.

Together, these give layered protection (network → identity → quota → cache) plus the
visibility needed to know who is calling the API, how much, and how it's performing — without
any of that logic living in the backend code itself.
