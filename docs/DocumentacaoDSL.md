# 📦 Operações Disponíveis (DSL)

A DSL permite manipular instâncias de `DataFrame` de forma declarativa e modular. Cada função recebe um `DataFrame`, executa uma sequência de operações e retorna o resultado para a próxima etapa do pipeline.

---

# 🛠️ Manipulação de Dados

Operações responsáveis pela manipulação da estrutura do DataFrame.

| Operação | Descrição |
|----------|-----------|
| `Converter` | Converte o tipo de uma coluna (`single`, `string`, `boolean`, `datetime`). |
| `Select` | Mantém apenas as colunas especificadas. |
| `Drop` | Remove uma ou mais colunas. |
| `Rename` | Renomeia uma coluna existente. |
| `Create` | Cria uma nova coluna utilizando um valor constante ou outro DataFrame. |
| `Copy` | Cria uma cópia independente do DataFrame. |

### Exemplo

```csharp
ManipulacaoDados()
{
df = df.Converter(col="GastoTotal", type="single")

df = df.Select(col="GastoTotal")

df = df.Drop(col=["StockCode","Description"])

df = df.Rename(col="GastoTotal", name="NovaColuna")

df = df.Create(name="NovaColuna", value=5, type="single")

dfNovo = df.Copy()

return df
}
```

---

# 🧹 Limpeza de Dados

Operações voltadas ao tratamento de valores ausentes, duplicados e filtragem.

| Operação | Descrição |
|----------|-----------|
| `DropDuplicates` | Remove registros duplicados. |
| `DropNa` | Remove linhas contendo valores nulos. |
| `FillNa` | Preenche valores nulos utilizando uma constante ou valor calculado. |
| `Replace` | Substitui valores específicos em uma coluna. |
| `Filter` | Filtra registros através de expressões lógicas. |

### Exemplo

```csharp
LimpezaDados()
{
df = df.DropNa()

df = df.DropDuplicates()

media = df.Mean(col="TaxaDevolucao")

df = df.FillNa(col="ColunaNula", value=media)

df = df.Replace(col="TaxaDevolucao", old="0", value="1")

df = df.Filter(condition="(GastoTotal > 1000) && (GastoTotal < 5000)")

return df
}
```

---

# 🧮 Operações Aritméticas

Permitem operações entre colunas ou entre uma coluna e um valor constante.

| Operação | Descrição |
|----------|-----------|
| `Sum` | Soma duas colunas ou uma coluna e um valor constante. |
| `Sub` | Calcula a diferença entre duas colunas ou entre uma coluna e um valor constante. |
| `Mult` | Multiplica duas colunas ou uma coluna por um valor constante. |
| `Div` | Divide uma coluna por outra ou por um valor constante. |
| `Mod` | Calcula o resto da divisão de uma coluna por um valor constante. |


### Exemplo

```csharp
OperacoesAritmeticas()
{
df = df.Sum(left="GastoTotal", right="GastoTotal", exit="Soma")

df = df.Sub(left="GastoTotal", right="GastoMensal", exit="Subtracao")

df = df.Mult(left="GastoTotal", right="TaxaDevolucao", exit="Multiplicacao")

df = df.Div(left="GastoTotal", right="GastoMensal", exit="Divisao")

df = df.Mod(col="GastoTotal", value=10, exit="Mod")

return df
}
```

---

# 📈 Operações Matemáticas

Transformações matemáticas aplicadas sobre uma única coluna.

| Operação | Descrição |
|----------|-----------|
| `Exp` | Calcula a função exponencial com base no valor da coluna. |
| `Log` | Calcula o logaritmo natural com base no valor da coluna. |
| `Log10` | Calcula o logaritmo na base 10 com base no valor da coluna. |
| `Pow` | Eleva os valores da coluna a uma potência. |
| `Sqrt` | Calcula a raiz quadrada. |
| `Abs` | Retorna o valor absoluto. |
| `Round` | Arredonda os valores para um número específico de casas decimais. |
| `Floor` | Arredonda para o inteiro imediatamente inferior. |
| `Ceil` | Arredonda para o inteiro imediatamente superior. |

### Exemplo

```csharp
OperacoesMatematicas()
{
df = df.Exp(col="GastoMedio", value=5)

df = df.Log(col="DiferencaTicket")

df = df.Log(col="GastoMensal")

df = df.Pow(col="GastoTotal", value=2)

df = df.Sqrt(col="GastoTotal")

df = df.Abs(col="DiferencaTicket")

df = df.Round(col="GastoTotal", value=2)

df = df.Floor(col="Mes")

df = df.Ceil(col="MediaDiasCompra")

return df
}
```

---

# 📊 Operações Estatísticas

Operações utilizadas para obtenção de estatísticas sobre uma coluna.

| Operação | Descrição |
|----------|-----------|
| `Mean` | Calcula a média aritmética. |
| `Median` | Calcula a mediana. |
| `Mode` | Calcula a moda. |
| `Std` | Calcula o desvio padrão. |
| `Var` | Calcula a variância. |
| `Max` | Retorna o maior valor da coluna. |
| `Min` | Retorna o menor valor da coluna. |

### Exemplo

```csharp
Estatisticas()
{
media = df.Mean(col="TaxaDevolucao")

mediana = df.Median(col="GastoTotal")

moda = df.Mode(col="GastaoMensal")

desvio = df.Std(col="DiferencaTicket")

variancia = df.Var(col="GastoMedio")

maximo = df.Max(col="GastoTotal")

minimo = df.Min(col="GastoTotal")

return df
}
```

---

# 📊 Agrupamentos e Janelas

Operações responsáveis por agregações, ordenação, junções e janelas temporais.

| Operação | Descrição |
|----------|-----------|
| `GroupBy` | Agrupa registros utilizando funções de agregação (`sum`, `mean`, `min`, `max`, `std`, `diff`). |
| `Sort` | Ordena o DataFrame pelas colunas especificadas. |
| `Merge` | Realiza a junção entre dois DataFrames. |
| `GroupWindow` | Executa operações de janela temporal dentro de grupos. |

### Exemplo

```csharp
AgrupamentosEJanelas()
{
df = df.GroupBy(col="CustomerID", agg="sum")

df = df.Sort(col="GastoTotal", asc="true")

df = df.GroupWindow(col="CustomerID", agg="diff", exit="DiferencaDias")

dfNovo = df.Copy()

df = df.Merge(right="dfNovo", on="CustomerID")

return df
}
```

---

# ⚡ Feature Engineering (`Map`)

O operador `Map` permite criar transformações linha a linha utilizando uma linguagem própria.

Diferentemente das operações tradicionais da DSL, o conteúdo do `Map` é convertido em **Expression Trees**, gerando funções compiladas em tempo de execução para execução direta sobre o `DataFrame`.

## Expressões

```csharp
Map()
{
df = df.Map(lambdax=[line:"x = GastoTotal * 2", line:"GastoMensal = x"])

return df
}
```

---

## Estruturas Condicionais

```csharp
Map()
{
df = df.Map(lambdax=[if:{condition:"(GastoTotal > 1000) || (GastoTotal == 1)", line:"GastoTotal = GastoTotal + 10", else:{line:"GastoTotal = 0"}}])

return df
}
```

---

## Laços de Repetição

```csharp
Map()
{
df = df.Map(lambdax=[for:{loop:"i = 0; i < 3; i = i + 1", line:"GastoTotal = GastoTotal + i"}])

return df
}
```

Cada Expressão pode ser colocada dentro de outro, no caso para for, if e else, como por exemplo

```csharp
ExpressoesDentroOutras()
{
df = df.Map(lambdax=[for:{loop:"i = 0; i < 100; i = i + 1", if:{condition:"(GastoTotal>1000) || (GastoTotal==1)", line:"GastoTotal=GastoTotal+10", else:{line:"GastoTotal=0"}}}])

return df
}
```

---

# 🚀 Exemplo de Pipeline

Os módulos da DSL podem ser organizados de forma sequencial para construir pipelines completos de preparação de dados.

```text
LimpezaColunas()
        ↓
Reordenacao()
        ↓
UltimaCompraDias()
        ↓
MediaCompraDias()
        ↓
DiferencaUltimoDiaCompra()
        ↓
TaxaDevolucao()
        ↓
GastoTotal()
        ↓
GastoMensal()
        ↓
GastoMedio()
        ↓
DiferencaTicket()
        ↓
TicketMedio()
        ↓
TicketDesvio()
        ↓
MergeFinal()
```

Cada função produz um novo `DataFrame`, que pode ser reutilizado pelas próximas etapas do pipeline, permitindo a construção de fluxos de preparação de dados modulares e reutilizáveis.

Também é possível encadear funções um atrás do outro, como por exemplo

```text
LimpezaColunas()
{
df = df.Converter(col="GastoTotal", type="single").Converter(col="GastoMensal", type="single").Converter(col="TaxaDevolucao", type="single").Converter(col="GastoMedio", type="single").Converter(col="DiferencaTicketMedio",type="single").Converter(col="DiferencaTicketDesvio", type="single")

return df
}

```
