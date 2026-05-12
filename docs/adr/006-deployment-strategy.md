# ADR 006 — Deployment Strategy

## Status
Proposed

## Context
The application requires a deployment strategy that supports scalability, resilience, and environment parity between development and production. The stack includes a .NET 8 WebApi, PostgreSQL, and RabbitMQ.

## Decision
We adopt a two-tier deployment model:

**Development / Local:**
- `docker-compose.yml` orchestrates all services locally
- Single-node, no replication, no persistent volume guarantees

**Production:**
- Container orchestration via **Kubernetes (k8s)**
- Each service (WebApi, PostgreSQL, RabbitMQ) runs as an independent Deployment or StatefulSet
- Horizontal Pod Autoscaler (HPA) scales the WebApi based on CPU/memory metrics
- Secrets managed via Kubernetes Secrets (or external vault — e.g., HashiCorp Vault, AWS Secrets Manager)
- Health checks via `/health` liveness and readiness probes

Recommended k8s resource layout:

```
k8s/
├── namespace.yaml
├── webapi/
│   ├── deployment.yaml      # 2+ replicas, HPA enabled
│   ├── service.yaml         # ClusterIP + Ingress
│   └── configmap.yaml       # Non-secret environment config
├── postgres/
│   ├── statefulset.yaml     # Single primary, PVC for data
│   └── service.yaml
└── rabbitmq/
    ├── statefulset.yaml     # rabbitmq:3.13-management-alpine
    └── service.yaml
```

## Consequences
**Positive:**
- docker-compose keeps local setup simple (one command)
- k8s enables zero-downtime deployments (rolling updates), auto-healing, and horizontal scaling
- WebApi is stateless — scales horizontally without coordination
- RabbitMQ and PostgreSQL are stateful — isolated as StatefulSets with persistent volumes

**Negative:**
- k8s adds operational complexity (cluster management, RBAC, networking)
- Requires a CI/CD pipeline to build and push Docker images before deployment
- Database migrations must be handled carefully in rolling deployments (backward-compatible migrations first)
