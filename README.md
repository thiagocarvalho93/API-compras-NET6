# Tutorial API dotnet core 6

Este é o exemplo de uma API capaz de cadastrar, deletar, alterar e consultar pessoas, produtos e compras. Baseado no tutorial [Criando uma API robusta em dotnet core 6](https://www.youtube.com/watch?v=ufjRbiaoou4&list=PLP4r6dpm_h-vPhZ-OXz3B5dcKpohAjhUE) do canal Manual do Programador.

## Seções:

1. [Softwares utilizados](#1-softwares-utilizados)
2. [Estrutura do projeto](#2-estrutura-do-projeto)
3. [Criação das entidades](#3-criação-das-entidades)
4. [Criação do banco de dados SQL Server](#3-criação-das-entidades)
5. [DbContext e mapeamento de entidades](#5-dbcontext-e-mapeamento-de-entidades)
6. [Repositories](#6-repositories)
7. [DTOs](#7-dtos)
8. [Services](#8-services)
9. [Injeção de dependências](#9-injeção-de-dependências)
10. [Controllers](#10-controllers)
11. [Paginação](#11-paginação)
12. [Unit of work](#12-unit-of-work)
13. [Autenticação JWT](#13-autenticação-jwt)

---

## 1. Softwares utilizados

[SDK do dotnet 6](https://dotnet.microsoft.com/en-us/download)

[VS code](https://code.visualstudio.com/download)

Extensões do vscode:

- C#: obrigatório pra compilar projetos em C#.
- C# Extensions: praticidade para criar novas classes, interfaces, controllers, etc. em C#.
- C# snippets: snippets úteis para C#.
- vsode-solution-explorer: facilita o trabalho com as solutions (adicionar referências, etc.).

[SQL Server 2017](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads)

[SQL Server Management Studio 18](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16)

Obs: Pode-se utilizar outro editor de texto (ex. Visual Studio) no lugar do VSCode e outro banco de dados (ex. Postgresql) e respectivo SGBD (ex. pgAdmin 4) no lugar do SQL Server. Fica a critério do usuário.

## 2. Estrutura do projeto

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

Obs: A camada API.Infra.IoC (Inversion of Control) é uma camada auxiliar responsável pela injeção de dependência, criada para que a camada API não dependa da camada API.Infra.Data. [Ver sobre Dependency injection](https://learn.microsoft.com/en-us/dotnet/architecture/maui/dependency-injection).

[Fonte](https://www.c-sharpcorner.com/article/introduction-to-clean-architecture-and-implementation-with-asp-net-core/)

## 3. Criação das entidades

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

Para a validação do modelo, primeiro criou-se a exception personalizada [DomainValidationException](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/ApiDotnet.Domain/Validations/DomainValidationException.cs), que herda da classe Exception. O método When já atribui a condição e a mensagem de erro como parâmetros, economizando assim uma linha de código condicional para cada validação.

Voltando ao modelo de [Person](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/ApiDotnet.Domain/Entities/Person.cs), foi criado um método com as validações dos campos que são entrados como atributos. Esse método de validação é chamado nos construtores, para não permitir que se criem instâncias inválidas do mesmo. A mesma lógica é aplicada aos modelos de [Product](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/ApiDotnet.Domain/Entities/Product.cs) e [Purchase](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/ApiDotnet.Domain/Entities/Purchase.cs).

## 4. Criação do banco de dados SQL Server

Para a criação do banco SQL Server, utilizou-se o software SQL Server Management Studio. Para a criação da database:

1. Na janela Object Explorer clique em ➕ para expandir a conexão
2. Botão direito na pasta databases e selecione new database
3. Escolher o nome que quiser e pressione OK
4. Pressione o botão direito novamente e selecione Refresh.

Agora para criar nossas tabelas pressione o botão direito na database criada e selecione New query. Uma nova janela se abrirá, onde colocaremos nosso código SQL. Para consultar o código utilizado nesse exemplo, verifique o arquivo [DDL.sql](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/DDL.sql).

Obs: no tutorial original utilizou-se o Postgresql e o pgAdmin 4, o tipo de banco de dados fica ao critério do usuário. No entanto, deve-se atentar à instalação do respectivo driver de conexão e configurações específicas do banco descritas nos próximos passos.

## 5. DbContext e mapeamento de entidades

O DbContext é a classe responsável por fazer a comunicação direta com o banco de dados. Para criar a conexão com o banco, será necessário a instalação de alguns packages. Em Infra.Data, adicionaremos os pacotes que comunicam com o SqlServer:

> caso queira usar outro banco de dados, procurar os pacotes correspondentes.

`dotnet add package Microsoft.EntityFrameworkCore.SqlServer`<br>
`dotnet add package Microsoft.EntityFrameworkCore.Design`<br>
`dotnet add package Microsoft.EntityFrameworkCore`

Em seguida, criamos a pasta Context em Infrastructure.Data e a classe [ApplicationDbContext](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/ApiDotnet.Infra.Data/Context/ApplicationDbContext.cs) herdando DbContext do EntityFramework.

```
└── Infrastructure.Data
    └── Context
        └── ApplicationDbContext.cs
```

Nessa classe, iremos declarar como atributos do tipo DbSet os nossos Models criados de Person, Product e Purchase, indicando que essas classes representam tabelas no banco de dados. Em seguida, fazemos um override do método OnModelCreating, para que as futuras configurações de mapeamento sejam aplicadas efetivamente.

No entanto, ainda há um problema: as tabelas não possuem os mesmos nomes nas entidades criadas e no banco de dados, bem como seus atributos e colunas. Como fazer para que o programa entenda qual atributo corresponde a qual coluna? Para isso, iremos criar o mapeamento entre elas. Essas configurações poderiam ser criadas no método OnModelCreating, mas para melhor organização, podemos criar a pasta Maps e [adicionar classes de mapeamento separadas para cada entidade](https://learn.microsoft.com/en-us/ef/core/modeling/), desde que implementem a interface IEntityTypeConfiguration&lt;TEntity>.

```
└── Infrastructure.Data
    └── Maps
        ├── PersonMap.cs
        ├── ProductMap.cs
        └── PurchaseMap.cs
```

Obs: repare que essas configurações de mapeamento serão aplicadas no override do método OnModelCreating na linha:<br>

```
modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
```

Tomaremos como exemplo a classe [PersonMap](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/ApiDotnet.Infra.Data/Maps/PersonMap.cs). As configurações são feitas no método Configure, que leva como parâmetro o EntityTypeBuilder do modelo. As configurações são feitas então adicionando os métodos ao builder.

```
public class PersonMap : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Pessoa");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("IdPessoa")
                .UseIdentityColumn();

            builder.Property(c => c.Name)
                .HasColumnName("NmPessoa");
            ...
```

Todos os métodos de configuração podem ser vistos [aqui](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.entitytypebuilder-1?view=efcore-6.0), mas os principais são:

- ToTable("Pessoa"): especifica o nome da tabela no banco.
- HasKey(c => c.Id): especifica a propriedade que é chave primária no banco.
- Property(c => c.Name): Para se referenciar à propriedade.
- HasColumnName("IdPessoa"): especifica o nome da coluna no banco da propriedade referenciada.
- UseIdentityColumn(): Para se referenciar à coluna da chave primária.

**Para mapear o relacionamento entre tabelas**

- HasOne ou HasMany identifica a propriedade de navegação no tipo de entidade em que você está iniciando a configuração.
- Em seguida, encadeia uma chamada para WithOne ou WithMany para identificar a navegação inversa.
- HasOne/WithOne são usados para propriedades de navegação de referência e HasMany/WithMany são usados para propriedades de navegação de coleção.
- HasForeignKey especifica a propriedade que será usada como chave estrangeira na tabela referenciada.

No nosso caso, teremos em [PersonMap](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/ApiDotnet.Infra.Data/Maps/PersonMap.cs):

```
public void Configure(EntityTypeBuilder<Person> builder)
        {
            ...
            builder.HasMany(c => c.Purchases)
                .WithOne(p => p.Person)
                .HasForeignKey(c => c.PersonId);
        }
```

Em [PurchaseMap](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/ApiDotnet.Infra.Data/Maps/PurchaseMap.cs) teremos a relação inversa:

```
public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            ...
            builder.HasOne(x => x.Person)
                .WithMany(p => p.Purchases);

            builder.HasOne(x => x.Product)
                .WithMany(p => p.Purchases);
        }
```

[Ver mais sobre mapeamento de relacionamentos](https://learn.microsoft.com/pt-br/ef/core/modeling/relationships?source=recommendations&tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key)

## 6. Repositories

Repositories são classes que encapsulam a lógica de acesso ao banco de dados.

Primeiro, criamos a pasta Repositories na camada Domain. Em seguida as interfaces IPersonRepository, IProductRepository e IPurchaseRepository (é uma boa prática em C# nomear interfaces com I no começo do nome).

```
└── Domain
    └── Repositories
        ├── IPersonRepository.cs
        ├── IProductRepository.cs
        └── IPurchaseRepository.cs
```

(...)

## 7. DTOs

Data Transfer Object (DTO) ou simplesmente Transfer Object é um padrão de projetos para o transporte de dados entre diferentes componentes de um sistema, diferentes instâncias ou processos de um sistema distribuído ou diferentes sistemas via serialização. A ideia consiste basicamente em agrupar um conjunto de atributos numa classe simples de forma a otimizar a comunicação. Numa chamada remota, seria ineficiente passar cada atributo individualmente. Da mesma forma seria ineficiente ou até causaria erros passar uma entidade mais complexa.

No nosso exemplo, na camada Application, criamos a pasta DTOs e as classes de DTOs relativos às entidades. Em seguida, na pasta DTOs criamos a pasta validations e as classes de validação correspondentes.

```
└── Application
    └── DTOs
        ├── PersonDTO.cs
        ├── ProductDTO.cs
        ├── PurchaseDTO.cs
        └── Validations
            ├── PersonDTOValidation.cs
            ├── ProductDTOValidation.cs
            └── PurchaseDTOValidation.cs


```

Fazemos agora a seguinte pergunta: o que queremos passar de dados para cada requisição e o que queremos receber? Por exemplo, poderíamos adicionar uma compra passando apenas o documento da pessoa e o código Erp do produto, ao invés de seus ids. PurchaseDTO ficaria da seguinte maneira então:

```
            public class PurchaseDTO
            {
                public string CodErp { get; set; }
                public string Document { get; set; }
            }

```

(TODO melhorar futuramente com DTOs personalizados para request e response.)

Para fazer as validações dos DTOs, adicionamos o package FluentValidation ([documentação](https://docs.fluentvalidation.net/en/latest/)) com `dotnet add package FluentValidation`. Tomando a classe PersonDTOValidation como exemplo, fazemos ela herdar de AbstractValidator&lt;PersonDTO>. No construtor, adicionamos as regras de validação com o método RuleFor:

```
            RuleFor(x => x.Document)
                .NotEmpty()
                .NotNull()
                .WithMessage("Documento deve ser informado!");

```

- RuleFor: Seleciona a propriedade a ser validada.
- NotEmpty e NotNull: regras de validação (não pode ser vazia nem nula).
- WithMessage: mensagem mostrada caso não passe na validação.

Para fazer as conversões entre os tipos de DTO e entidades, adicionamos o pacote AutoMapper ([documentação](https://docs.automapper.org/en/stable/Getting-started.html)) com `dotnet add package AutoMapper`.

(...)

## 8. Services

As classes service encapsulam a regra de negócios da aplicação, controlando as transações e coordenando as respostas na implementação de suas operações.

Primeiro, criamos a pasta Services na camada Application. Dentro dela, criamos as classes de Service de cada uma das entidades, e também outra pasta Interfaces, que conterá as interfaces dos services de cada uma delas. Criamos também uma classe ResultService, que fará a representação das respostas das implementações das operações.

```
└── Application
    └── Services
        ├── PersonService.cs
        ├── ProductService.cs
        ├── PurchaseService.cs
        ├── ResultService.cs
        └── Interfaces
            ├── IPersonService.cs
            ├── IProductService.cs
            └── IPurchaseService.cs


```

(...)

## 9. Injeção de dependências

Ao especificar dependências como tipos de interface, a injeção de dependência permite desacoplar os tipos concretos do código que depende desses tipos. Ele geralmente usa um contêiner que contém uma lista de registros e mapeamentos entre interfaces e tipos abstratos e os tipos concretos que implementam ou estendem esses tipos.

Criaremos esse contêiner criando a classe [DependencyInjection.cs](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/ApiDotnet.Infra.IoC/DependencyInjection.cs) na camada Infra.IoC. Essa classe **precisa ser estática**, pois iremos utilizar extension method.

```
└── Infra.IoC
    └── DependencyInjection.cs
```

Nesse exemplo, foi criado um [extension Method](https://weblogs.asp.net/scottgu/new-orcas-language-feature-extension-methods) (para adicionar um novo método à interface IServiceCollection) AddServices. Esse método será chamado posteriormente em [Program.cs](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/ApiDotnet.Api/Program.cs). O parâmetro IConfiguration é passado pra configurar a conexão do banco de dados. O método terá a assinatura `public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)`.

Para injetar o DbContext, deve-se explicitar suas configurações, que serão feitas posteriormente.

```
            // DbContext
            services.AddDbContextPool<DbContext, ApplicationDbContext>(options =>
                        {
                            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                        });
```

Em seguida, adicionamos os mapeamentos entre as interfaces e tipos concretos dos repositories com o método AddScoped.

```
            // Repositories
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IPurchaseRepository, PurchaseRepository>();
            // Services
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPurchaseService, PurchaseService>();

```

Para fazer a injeção do AutoMapper usamos o método AddAutoMapper. No fim do método retornamos services.

```
            services.AddAutoMapper(typeof(DomainToDTOMapping));
            return services;
```

Ao final, não esquecer de adicionar a chamada do método em [Program.cs](https://github.com/thiagocarvalho93/API-compras-NET6/blob/main/ApiDotnet.Api/Program.cs).

```
var builder = WebApplication.CreateBuilder(args);

...

builder.Services.AddServices(builder.Configuration);
```

## 10. Controllers

A camada de controllers é a camada que permite ao usuário se comunicar com a API através do protocolo HTTP (...)

## 11. Paginação

Para se fazer uma busca dos registros do banco e não sobrecarregar o usuário com excesso de dados, recomenda-se a utilização da busca paginada (...)

## 12. Unit of work

Existem casos nos quais uma requisição modifica o banco de dados mais de uma vez. Nesse caso, a aplicação pode modificar algumas tabelas no banco e ocorrer um erro no meio do caminho, ocasionando dados errados. Para isso, (...)

## 13. Autenticação JWT

Para restringir o acesso de métodos a certos usuários cadastradaos (...)
