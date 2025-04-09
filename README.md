<h1 align="center" style="font-weight: bold;">SIG</h1>

<p align="center">
    <a href="#tech">Tecnologias</a> 
    <a href="#started">Começando</a>
    <a href="#routes">API Endpoints</a>
    <a href="#contribute">Contribuição</a>
</p>

<p align="center">
    <b>SIG é uma API simples para o controle de atividades de uma escola</b>
</p>

<h2 id="tech">💻 Tecnologias</h2>

- .NET 8
- C#
- Asp .NET Core
- Entity Framework
- AutoMapper
- FluentValidation
- Postgres
- Xunit
- Moq

<h2 id="started">🚀 Começando</h2>

<h3>Pré-Requisitos</h3>

- .NET 8
- Postgres

<h3>Clonando o projeto</h3>

Como clonar o projeto

```bash
git clone https://github.com/ThiagoFBastos/SIG.git
```

<h3>Configurando appsettings.json</h2>

```
 "ConnectionStrings": {
    "DefaultConnection": "<SUA STRING DE CONEXÃO>",
    "TestConnection": "<SUA STRING DE CONEXÃO DE TESTE>
  }
```

<h3>Migrações</h3>

1. Baixe o comando dotnet ef: dotnet tool update --global dotnet-ef
2. Execute o comando para executar as migrações: dotnet ef --project Persistence --startup-project API database update --context RepositoryContext

<h3>Executando</h3>

Como executar o projeto

```bash
cd SIG
dot net run --project API
```

ou

Execute o projeto no Visual Studio

<h2 id="routes">📍 API Endpoints</h2>

- Execute o projeto e vá até a página: https://localhost:7208/swagger/index.html ou http://localhost:5139/swagger/index.html no seu navegador. Lá você irá encontrar a documentação da API.

<h2 id="contribute">📫 Contribuição</h2>

1. `git clone https://github.com/ThiagoFBastos/SIG.git`
2. `git checkout -b feature/NAME`
3. Siga os padrões de commit
4. Abra um Pull Request explicando o problema que está resolvendo

<h3>Documentações que podem ajudar</h3>

[📝 Como criar um Pull Request](https://www.atlassian.com/br/git/tutorials/making-a-pull-request)

[💾 Padrão de commit](https://gist.github.com/joshbuchea/6f47e86d2510bce28f8e7f42ae84c716)