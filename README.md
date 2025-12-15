# HR Manager – teste fullstack

Aplicação fullstack para gestão de colaboradores com API em .NET 8 (arquitetura hexagonal) e front em React. Inclui autenticação JWT, restrição de hierarquia (não cria cargo superior ao do usuário logado), validação de maioridade, documentação via Swagger e orquestração com Docker.

## Como rodar

### Via Docker (recomendado)
1. Certifique-se de ter Docker e Docker Compose instalados.
2. Na raiz, rode `docker-compose up --build`.
3. API: http://localhost:8080 (Swagger em `/swagger` em dev).  
   Front: http://localhost:4173  
   DB Postgres: localhost:5432 (user `hradmin` / password `changeme`).

### Manualmente
1. **Backend**  
   - Requer .NET 8 SDK e Postgres rodando local.  
   - Ajuste a connection string em `server/src/Api/appsettings.json` se necessário.  
   - Restaure e publique: `dotnet restore server/Server.sln` e `dotnet run --project server/src/Api/Api.csproj`.
2. **Frontend**  
   - Node 18+.  
   - `cd client && npm install && npm run dev` (porta 5173).  
   - Exponha `VITE_API_URL=http://localhost:8080` se a API estiver em outra origem.

## Credenciais seed
- Usuário diretor inicial: `admin@empresa.com` / `Trocar@123` (configurável em `Seed` do `appsettings`).

## API
- Autenticação JWT (`Authorization: Bearer <token>`).
- Endpoints principais:
  - `POST /api/auth/login`
  - `GET /api/employees`
  - `GET /api/employees/{id}`
  - `POST /api/employees`
  - `PUT /api/employees/{id}`
  - `DELETE /api/employees/{id}` (marca como inativo)
- Regras:
  - Não cria/atualiza colaborador com cargo superior ao usuário logado.
  - Exige maioridade e ao menos um telefone.
  - Documento e e-mail únicos.

## Estrutura
- `server/` – solução .NET (Domain, Application, Infrastructure, Api)
- `client/` – front React + Vite
- `docker-compose.yml` – API, DB Postgres e front

## Testes
- Unitários (validador de colaborador): `dotnet test server/tests/Application.Tests/Application.Tests.csproj`

## Observações
- Ambiente local aqui só possui SDK 6; não executei build/testes. Ao rodar com .NET 8 + Postgres, a aplicação deve subir normalmente. Ajuste a secret JWT (`Jwt:SecretKey`) em produção.
