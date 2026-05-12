# RabbitMQ Integration

## Configuração

Este projeto usa **Rebus** com **RabbitMQ** para publicação de domain events.

### Domain Events Publicados

- `SaleCreatedEvent` - Disparado quando uma venda é criada
- `SaleModifiedEvent` - Disparado quando uma venda é modificada
- `SaleCancelledEvent` - Disparado quando uma venda é cancelada
- `ItemCancelledEvent` - Disparado quando um item é cancelado

### Executar com Docker Compose

```bash
docker-compose up -d
```

Isso iniciará:

- **PostgreSQL** (porta 5432)
- **RabbitMQ** (portas 5672 e 15672)
- **WebAPI** (porta 8080)

### RabbitMQ Management UI

Acesse: http://localhost:15672

**Credenciais:**

- Username: `developer`
- Password: `ev@luAt10n`

### Executar Localmente (Sem Docker)

1. **Inicie o RabbitMQ localmente:**

```bash
docker run -d --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  -e RABBITMQ_DEFAULT_USER=developer \
  -e RABBITMQ_DEFAULT_PASS=ev@luAt10n \
  rabbitmq:3.13-management-alpine
```

2. **Execute a aplicação:**

```bash
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

### Executar Sem RabbitMQ (Fallback)

Se o RabbitMQ não estiver disponível, a aplicação automaticamente usará `LogEventPublisher`, que apenas registra os eventos no Serilog sem falhas.

Para desabilitar completamente o RabbitMQ, remova ou comente a connection string no `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
    // "RabbitMq": "amqp://developer:ev@luAt10n@localhost:5672"
  }
}
```

### Monitoramento de Eventos

Os eventos são publicados na fila `sales-management`. Você pode visualizar as mensagens na Management UI em:

**Queues** → `sales-management` → **Get Messages**

### Testes

Os testes (Integration e Functional) automaticamente usam `LogEventPublisher` em vez do `RebusEventPublisher`, portanto **não requerem RabbitMQ** para executar.

```bash
dotnet test
```

### Implementação

- **Publisher:** `RebusEventPublisher` (com Polly retry 3x)
- **Fallback:** `LogEventPublisher` (sempre disponível)
- **Transport:** RabbitMQ (quando configurado)
- **Queue:** `sales-management`

### Retry Policy

Todas as publicações de eventos têm retry automático com **backoff exponencial**:

- Tentativa 1: Imediata
- Tentativa 2: Após 2 segundos
- Tentativa 3: Após 4 segundos

Se todas as tentativas falharem, o evento é apenas registrado como warning (não falha a operação).
