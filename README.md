# AVE 2020/2021 FireMapper LI41D - G07

## Geral

Este projeto foi dividido em duas fases:
1. Trabalho Prático 1 -> Reflexão
2. Trabalho Prático 2 -> Classes geradas dinâmicamente

O projeto foi construído da seguinte forma: 

<ol>
<li>Attributes:
<ul>
<li>FireCollection: Contém o nome da coleção de armazenamento, sendo aplicável à classe representante do domínio.</li>
<li>FireKey: Aplicável à propriedade representante da chave primária.</li>
<li>FireIgnore: Aplicável a propriedades não correspondentes ao domínio, ou seja que não são para mapear.</li>
</ul>
</li>
<li>Mappers:
<ul>
<li>IDataMapper: Interface representante de um mapeador de dados.</li>
<li>BaseFireMapper: Classe que implementa IDataMapper responsável por servir de base ás classes abaixo na hierarquia de tipos.</li>
<li>FireDataMapper: Classe que extende BaseFireMapper e que usa reflexão para obter os valores das propriedades.</li>
<li>DynamicFireMapper: Classe que extende BaseFireMapper e que usa código gerado dinâmicamente para obter os valores das propriedades.</li>
</ul>
</li>
<li>Wrappers:
<ul>
<li>IPropertyWrapper: Interface representante de uma propriedade.</li>
<li>PropertyPrimitive: Classe que implementa IPropertyWrapper responsável por abstrair propriedades primitivas.</li>
<li>PropertyComplex: Classe que implementa IPropertyWrapper responsável por abstrair propriedades complexas.</li>
<li>WrapperBuilder: Classe responsável por gerar dinâmicamente tipos que implementem IPropertyWrapper.</li>
</ul>
</li>
<li>Sources:
<ul>
<li>IDataSource: Interface representante de uma instância de armazenamento.</li>
<li>FireDataSource: Classe que implementa IDataSource responsável por abstrair o armazenameto no serviço FireStore da Google.</li>
<li>WeakDataSource: Classe que implementa IDataSource responsável por originar um armazenamento local.</li>
<li>NonExecutableSource: Classe que implementa IDataSource responsável por não executar operaçãoes de escrita, somente de leitura.</li>
</ul>
</li>
</ol>


## Trabalho Prático 1

**DATA LIMITE de entrega**: 18 de Abril de 2021. Fazer _push_ a tag `trab1` no repositório do grupo.

Neste trabalho pretende-se desenvolver a biblioteca _FireMapper_ que
disponibiliza uma abstração sobre uma **colecção** de uma base de dados
documental FireStore. Em modo resumido, o FireStore é uma base de dados NoSQL
que armazena **colecções** de **documentos** JSON.

https://github.com/isel-leic-ave/FireMapper

Uma base de dados FireStore é gerida a partir de um projecto FireBase. 
**De modo a se familiarizar com a tecnologia deve em primeiro lugar seguir os
passos do guião 
“[_FireStore get started_](https://github.com/isel-leic-ave/FireMapper/blob/master/isel-AVE-2021-FireStore-get-started.md)”**
para criar alguns documentos e uma pequena aplicação em C# que se liga a essa
base de dados listando o seu conteúdo na consola.

O objectivo da biblioteca _FireMapper_ é facilitar o acesso aos **documentos**
de uma **colecção** por via de um `IDataMapper`. Esta interface especifica os
métodos de acesso à **coleção** e que correspondem às operações CRUD.

```csharp
    public interface IDataMapper
    {
        IEnumerable GetAll();
        object GetById(object keyValue);
        void Add(object obj);
        void Update(object obj);
        void Delete(object keyValue);
    }
```

Por cada colecção deve existir uma **classe de domínio** com propriedades
correspondentes às propriedades de um **documento**. Essas classes podem ter
informação complementar dada na forma de anotações, por via dos seguintes
_custom attributes_:
* `FireCollection` - aplicado a uma classe para identificar o nome da coleção Firestore.
* `FireKey` - identifica a propriedade que é chave única na pesquisa de um documento através do método `GetById`
* `FireIgnore` - propriedade a ignorar no mapeamento com um documento.

Exemplo:

<table>
<tr>
<td>

```csharp
[FireCollection("Students")]
public record Student(
    [property:FireKey] string Number,
    string Name, 
    [property:FireIgnore] string Classroom)  
{}
```
 
</td>
<td>

Students collection: 
```json
{
    "Name": "Zanda Cantanda",
    "Number": "72538",
    "Classroom": "TLI41D"
}
```

</td>
</tr>
</table>

A classe `FireDataMapper` implementa a interface `IDataMapper` com um
comportamento dependente da classe de domínio (e.g. `Student`), cujo `Type` é
fornecido na sua instanciação.

```csharp
IDataMapper studentsMapper = new FireDataMapper(typeof(Student), ...);
```

A implementação de `FileDataMapper` deve ser feita com o suporte da classe
`FireDataSource` da biblioteca _FireSource_ disponibilizado no respectivo
projecto que integra a solução.


Enquanto a classe `FireDataSource` lida com dados fracamente tipificados na
forma de `Dictionary<string, object>`, a classe `FireDataMapper` trata objectos
de domínio, e.g. instâncias de `Student`.


1. Implemente os _custom attributes_ `FireCollection`, `FireKey` e `FireIgnore`.

1. Usando a API de Reflexão implemente a classe `FireDataMapper` que faz o
   mapeamento entre objectos de domínio e dados na forma de 
   `Dictionary<string, object>` manipulados por uma instância de `IDataSource`.
   Implemente os testes unitários que validem o correcto funcionamento dos métodos,
   incluíndo casos de excepção como por exemplo, ausência de anotações; mais que uma
   propriedade anotada com `FireKey`; etc.
   Garanta o máximo de cobertura observando o _coverage report_ obtido através do 
   procedimento descrito no README.md.

1. Faça uma implementação alternativa de `IDataSource` na classe
   `WeakDataSource` que mantém os dados apenas em memória (defina a estrutura de
   dados ao seu critério). Valide o funcionamento com testes unitários. Note que
   a classe `FireDataMapper` pode funcionar com qualquer implementação de
   `IDataSource` especificada por parâmetro do construtor.

1. Defina as classes de um modelo de domínio e crie uma nova base de dados para
   esse modelo no FireStore e teste com a sua biblioteca _FireMapper_. Exemplos:
   carros, filmes, música, desportos, jogadores de futebol, ligas de futebol,
   jogadores da NBA, videogames, surfistas, lutadores, séries de TV, surf spots,
   praias, cidades do mundo, resorts de neve, hotéis, marcas, etc.
   **Requisitos**:
   * O modelo definido deverá incluir no mínimo duas entidades (classes) com uma
     relação de associação. 
   * Use _auto id_ nos documentos da base de dados.
   * Veja o exemplo de associação criado nos testes do projecto
     _FireSource.Tests_ e como a propriedade `Classroom` de um documento
     `Student` corresponde à propriedade `Token` de um documento `Classroom`
     semelhante ao comportamento de uma _foreign key_.
   * Cada grupo de trabalho deverá usar um modelo de domínio distinto.
   * **Modelos de domínio mais ricos em termos de dados e relações entre si, serão valorizados.**

Faça um _pull request_ para o repositório
https://github.com/isel-leic-ave/FireMapper/ para adicionar um novo ficheiro na
pasta `Db` que identifica o modelo domínio. Cada grupo deve escolher um domínio
diferente. Os domínios são atribuídos de acordo com a ordem dos _pull request_.

Cada _pull request_ deve atender aos seguintes requisitos:
* Um novo ficheiro com o nome na forma: `<turma>`-`<grupo>`-nome-do-dominio.txt,
  por exemplo: i41d-g07-surf-spots.txt
* O conteúdo do ficheiro deve descrever o esquema (nomes de campo e tipo) de
  cada coleção. Deve ter pelo menos duas coleções distintas com uma associação
  (_foreign key_) entre elas.
* Um exemplo dos documentos.

5. `FireDataMapper` deve suportar classes de domínio com propriedades de tipo
   definido por outras classes de domínio. Neste caso deve criar uma outra
   instância de `FireDataMapper` auxiliar para o respectivo tipo da propriedade
   que permite aceder à respectiva colecção. Valide o funcionamento da
   associação em testes unitários.

Exemplo  a classe `ClassroomInfo` correspondente ao tipo da propriedade `Classroom`:

```csharp
[FireCollection("Students")]
public record Student( [property:FireKey] string Number, string Name, ClassroomInfo Classroom)  {}

[FireCollection("Classrooms")]
public record ClassroomInfo([property:FireKey] string Token, string Teacher) {}
```

## Trabalho Prático 2

**DATA LIMITE de entrega**: 23 de Maio de 2021. Fazer _push_ a tag `trab3` no repositório do grupo.

**Objectivos**: Análise e manipulação programática de código intermédio com API
de `System.Reflection.Emit`.

No seguimento do Trabalho 1 desenvolvido na biblioteca **FireMapper**
pretende-se desenvolver uma nova classe `DynamicDataMapper` que implementa
`IDataMapper`, mas que ao contrário de `FireDataMapper` **NÃO usa reflexão no
acesso (leitura ou escrita) das propriedades das classes de domínio**. 
Note, que **continuará a ser usada reflexão na consulta** da
_metadata_, deixando apenas de ser usada reflexão em operações como
`<property>.SetValue(…)` ou `<property>.GetValue(…)`.
O acesso a propriedades passa a ser realizado directamente com base em código IL
emitido em tempo de execução através da API de `System.Reflection.Emit`. 

Para tal, `DynamicDataMapper` deve gerar em tempo de execução implementações, em
que **cada tipo** implementa o acesso a uma determinada propriedade de uma
classe de domínio.

### Etapa 0 - TPC06 - `System.Reflection`

Ainda usando apenas a Reflection API (sem Emit) reorganize o projecto resultante
do Trabalho 1 de modo a que `FireDataMapper` mantenha uma estrutura de dados com
um tipo de elemento (interface) que define a API de acesso a uma propriedade de
uma classe de domínio.

Esta interface deve ter implementações diferentes, consoante represente o acesso
a uma propriedade "simples" (_string_ ou primitivo) ou "complexa" (do tipo de
outra classe de domínio).


### Etapa 1 - `System.Reflection.Emit`

Implemente `DynamicFireMapper` que gera dinamicamente implementações da
interface definida na Etapa 0, para cada propriedade de cada classe de
domínio acedida.

Implemente testes unitários que validem o correcto funcionamento de
`DynamicFireMapper`.

**Requisito**: Deve incluir nos testes unitários a utilização de entidades de
domínio de tipo valor (i.e. definidas como `struct`).

***
### Abordagem de desenvolvimento

Como suporte ao desenvolvimento de `DynamicFireMapper` deve usar as ferramentas:
  * `ildasm`
  * `peverify`

Deve desenvolver em C# uma classe _dummy_ num projecto separado com uma
implementação semelhante aquela que pretende que seja gerada através da API de
`System.Reflection.Emit`. 
Compile a classe _dummy_ e use a ferramenta `ildasm` para visualizar as instruções
IL que servem de base ao que será emitido através da API de `System.Reflection.Emit`. 

Grave numa DLL as classes geradas pelo `DynamicFireMapper` e valide através da ferramenta 
`peverify` a correcção do código IL.

### Etapa 2 - Benchmarking

Implemente uma aplicação consola num outro projecto **FireMapperBench** da mesma
solução para comparar o desempenho dos métodos entre as classes `FireDataMapper`
e `DynamicDataMapper`.

Para as medições de desempenho **use a abordagem apresentada nas aulas**.
Registe e comente os desempenhos obtidos entre as duas abordagens. 

**Atenção:**
* **testes de desempenho NÃO são testes unitários**
* Use `WeakDataSource` nas medidas de desempenho.
* Evite IO.