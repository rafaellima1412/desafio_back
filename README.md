# 🛠️ O backend em minimal API:

**Requisitos Desenvolvidos:**
* login com a geração de token e proteção das rotas.
* Swagger
* tela device manager onde se lista os devices cadastrados
* tela de details onde usuario verifica os comandos disponiveis e resposta cadastrada.
  
**Ideias, Sugestões e implementações futuras:**
* Para acesso aos dados dos dispositivos pode se usar TcpCLient para se conectar a dispositivos telnet e obter os fluxos, enviar comandos do dispositivo,
  ler a resposta do dispositivo, armazenar a informação e fechar conexão.
* Para otimizar pode se guardar a resposta do dispositivo em memory cache para cada comando.
  Assumindo que cada comando possa produzir a mesma resposta.
  
**Algumas coisas foram abstraidas por falta de tempo**

* Por não ter uma camada de dados com uma base de dados de usuarios para ser acessada para validar usuarios cadastrados e
  verificar as hashs das senhas salvas.
  essa logica foi imlementada:
  // Authentication logic
if (loginRequest.Username == "admin" && loginRequest.Password == "admin"){...}
*Como  não havia  um device para testar a resposta do comando cadastrado essa parte ficou como ideia e sugestão futura.

# Projeto Poderia estar em DDD

*Onde seria dividido em:*
    Desafio.Domain
    Desafio.Application
    Desafio.Infrastructure
    Desafio.Presentation

Onde a pasta model com DeviceEntities e LoginRequest criada estaria na domain

A interface do repositorio e Interface do Serviço também estaria nessa camada:

A implementação da Intercafe do Serviço estaria na camada de Aplicação.
A impplementação da Inteface de Repositorio estaria na camada de InfraEstrutura.
Caso houvesse uma conexão com Banco via ORM EF ou Dapper seria configurado na camada de Dominio.
Na camada de Presentation estaria as controller passando por Dtos as informações vindas das camadas de dominio via Dtos configurados na camada de aplicação.

Poderia estar no parthen CQRS com os comands e querys para escalabilidade.

