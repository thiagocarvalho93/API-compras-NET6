# Tutorial API dotnet core 6

Este é o exemplo de uma API capaz de cadastrar, deletar, alterar e consultar pessoas, produtos e compras. Baseado no tutorial [Criando uma API robusta em dotnet core 6](https://www.youtube.com/watch?v=ufjRbiaoou4&list=PLP4r6dpm_h-vPhZ-OXz3B5dcKpohAjhUE) do canal Manual do Programador.

## Seções:

- Softwares utilizados
- Estrutura do projeto
- Criação das entidades
- Criação do banco de dados SQL Server
- DbContext e mapeamento de entidades
- Repositories
- DTOs
- Services
- Injeção de dependências
- Controllers
- Paginação
- Transação
- Autenticação JWT

---

## Softwares utilizados

SDK do dotnet 6. [Link](https://dotnet.microsoft.com/en-us/download)

VS code. [Link](https://code.visualstudio.com/download)

Extensões do vscode:

- C#: obrigatório pra compilar projetos em C#.
- C# Extensions: praticidade para criar novas classes, interfaces, controllers, etc. em C#.
- C# snippets: snippets úteis para C#.
- vsode-solution-explorer: facilita o trabalho com as solutions (adicionar referências, etc.).

SQL Server 2017. [Link](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads)

SQL Server Management Studio 18. [Link](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16)

## Estrutura do projeto

Uma boa arquitetura é vital para construir aplicações escaláveis, modulares e de fácil manutenção. Esse projeto utiliza a arquitetura "Clean code" (ou arquitetura limpa), que tem como principais benefícios ser:

- Independente do banco de dados e dos frameworks utilizados.
- Independente da camada de UI (interface do usuário).
- Altamente testável.

O principal conceito por trás desse padrão é que a lógica/código da aplicação mais improvável de ser alterada seja escrita sem nenhuma dependência externa. Então, se precisarmos mudar o banco de dados, por exemplo, o Core da aplicação (regras de negócio) não precisará ser alterado. Esse padrão é constituído basicamente de 4 camadas principais:

- Domain
- Application (juntamente com o Domain formam o Core da aplicação)
- Infrastructure
- Sistemas externos (Front end, database, etc.)

Na Arquitetura Limpa todas as dependências fluem de fora para dentro. O Core não depende de nenhuma outra camada e as camada de Infrastructure e dependências externas dependem do Core.

![image](https://csharpcorner-mindcrackerinc.netdna-ssl.com/article/introduction-to-clean-architecture-and-implementation-with-asp-net-core/Images/pic2-1.png)

Para começar o projeto, criou-se uma pasta com o nome ApiDotNet6 (ou outro nome a seu critério). Dentro da pasta, no VSCode, foi utlizado o console para criação dos projetos. Inicialmente, criou-se a solution pelo comando `dotnet new sln`. Esse comando gera uma nova solution com o nome da pasta na qual você está.

Em seguida, criou-se a WebApi pelo comando `dotnet new webapi -n API -o API`. Agora pra criar as class libs:<br>
`dotnet new classlib -n Domain -o API.Domain`<br>
`dotnet new classlib -n Infra.Data -o API.Infra.Data`<br>
`dotnet new classlib -n Infra.IoC -o API.Infra.IoC`<br>
`dotnet new classlib -n Application -o API.Application`<br>

Para vincular os projetos à solution:<br>
`dotnet sln add ./API/`<br>
`dotnet sln add ./API.Domain/`<br>
`dotnet sln add ./API.Infra.Data/`<br>
`dotnet sln add ./API.Infra.IoC/`<br>
`dotnet sln add ./API.Application/`<br>

Agora, para adicionar as referências, pode-se fazer de duas formas: através dos comandos `dotnet add [<PROJECT>] reference <PROJECT_REFERENCES>` ou pela extensão vsode-solution-explorer. As seguintes referências foram feitas:

- Domain não referencia ninguém, mas é referenciado por todos os outros projetos.
- Infra.IoC também referencia API.Application e API.Infra.Data.
- API também referencia API.Infra.IoC.

Rodar a build para verificar se há erros com `dotnet build`.

Obs: A camada API.Infra.IoC é uma camada auxiliar responsável pela injeção de dependência, criada para que a camada API não dependa da camada API.Infra.Data.

[Fonte](https://www.c-sharpcorner.com/article/introduction-to-clean-architecture-and-implementation-with-asp-net-core/)

## Criação das entidades

Os modelos são as classes responsáveis por representar as tabelas do banco de dados, onde cada um dos atributos da classe representa uma coluna de sua tabela correspondente. Elas são as classes principais do nosso projeto, portanto ficam na camada Domain.

No nosso caso, eles foram criados na pasta Entities (também pode ser chamada de Models). Além disso, foi criada uma classe de Exception específica (DomainValidationException.cs) para representar um erro na instanciação de uma Entity.

```
└── Domain
    └── Entities
        ├── Person.cs
        ├── Product.cs
        └── Purchases.cs
    └── Validations
        └── DomainValidationException.cs
```

Como foi dito inicialmente, essa API deve ser capaz de fazer operações CRUD de pessoas, produtos e compras. É importante observar os relacionamentos que cada uma das entidades terão entre si para fazer os Models. Nesse caso, teremos o seguinte:

```
     Person (1,n)-> Purchase <-(n,1) Product
```

Ou seja, uma pessoa ou um produto podem estar vinculada a n compras, mas uma compra está vinculada apenas a uma pessoa e a um produto. Um produto e uma pessoa não possuem nenhum relacionamento entre si. Tendo isso em mente, nossos models de Person e Product obrigatoriamente devem ter como atributo uma coleção de itens do tipo Purchase (no nosso caso optou-se pela interface ICollection) e a classe Purchase deve ter um Person e um Product. Por exemplo, para a classe [Person](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/ApiDotnet.Domain/Entities/Person.cs), pode-se observar os atributos e ao final a collection de purchases. Importante notar também que o construtor **deve inicializar uma lista vazia**, para evitar o problema de null pointer exception:

```
public sealed class Person
    {
        (...outros atributos)
        public ICollection<Purchase> Purchases { get; set; }

    public Person(string name, string document, string  phone)
        {
            (...)
            Purchases = new List<Purchase>();
        }
```

Adicionalmente
Para a validação do modelo, primeiro criou-se a exception personalizada [DomainValidationException](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/ApiDotnet.Domain/Validations/DomainValidationException.cs), que herda da classe Exception. O método When já atribui a condição e a mensagem de erro como parâmetros, economizando assim uma linha de código condicional para cada validação.

Voltando ao modelo de [Person](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/ApiDotnet.Domain/Entities/Person.cs), foi criado um método com as validações dos campos que são entrados como atributos. Esse método de validação é chamado nos construtores, para não permitir que se criem instâncias inválidas do mesmo. A mesma lógica é aplicada aos modelos de [Product](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/ApiDotnet.Domain/Entities/Product.cs) e [Purchase](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/ApiDotnet.Domain/Entities/Purchase.cs).

## Criação do banco de dados em SQL Server

Para a criação do banco SQL Server, utilizou-se o software SQL Server Management Studio. Para a criação da database: Janela Object Explore ➡ Clique em ➕ para expandir a conexão ➡ Botão direito na pasta databases e selecione new database ➡ Escolher o nome que quiser e pressione OK ➡ Pressione o botão direito novamente e selecione Refresh.

Agora para criar nossas tabelas pressione o botão direito na database criada e selecione New query. Uma nova janela se abrirá, onde colocaremos nosso código SQL. Para consultar o código utilizado nesse exemplo, verifique o arquivo [DDL.sql](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/DDL.sql).

## DbContext e mapeamento de entidades

O DbContext é a classe responsável por fazer a comunicação direta com o banco de dados. (...)

## Repositories

A camada de repositório contem os métodos de consulta e alteração do banco de dados. (...)

## DTOs

Data Transfer Objects (DTO) são objetos que representam (...)

## Services

A camada services contém as regras de negócio (...)

## Injeção de dependências

O pattern de injeção de dependência (dependency injection) é utilizado para (...)

## Controllers

A camada de controllers é a camada que permite ao usuário se comunicar com a API através do protocolo HTTP (...)

## Paginação

Para se fazer uma busca dos registros do banco e não sobrecarregar o usuário com excesso de dados, recomenda-se a utilização da busca paginada (...)

## Transação

Existem casos nos quais uma requisição modifica o banco de dados mais de uma vez. Nesse caso, a aplicação pode modificar algumas tabelas no banco e ocorrer um erro no meio do caminho, ocasionando dados errados. Para isso, (...)

## Autenticação JWT

Para restringir o acesso de métodos a certos usuários cadastradaos (...)
