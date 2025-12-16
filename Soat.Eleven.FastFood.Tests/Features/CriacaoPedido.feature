# language: pt-BR
Funcionalidade: Criação de Pedido
    Como um cliente do fast food
    Eu quero criar um pedido com itens
    Para que eu possa realizar minha compra

Cenário: Criar um pedido com sucesso
    Dado que eu tenho um token de atendimento válido
    E que eu tenho um cliente identificado
    E que eu tenho os seguintes itens no pedido:
        | ProdutoId                              | Quantidade | PrecoUnitario | DescontoUnitario |
        | 11111111-1111-1111-1111-111111111111   | 2          | 25.00         | 0.00             |
        | 22222222-2222-2222-2222-222222222222   | 1          | 15.00         | 2.50             |
    Quando eu criar o pedido
    Então o pedido deve ser criado com sucesso
    E o pedido deve ter status "Pendente"
    E o pedido deve ter uma senha gerada
    E o pedido deve conter 2 itens

Cenário: Criar um pedido sem cliente identificado
    Dado que eu tenho um token de atendimento válido
    E que eu não tenho um cliente identificado
    E que eu tenho os seguintes itens no pedido:
        | ProdutoId                              | Quantidade | PrecoUnitario | DescontoUnitario |
        | 11111111-1111-1111-1111-111111111111   | 1          | 30.00         | 0.00             |
    Quando eu criar o pedido
    Então o pedido deve ser criado com sucesso
    E o pedido deve ter status "Pendente"
    E o pedido deve ter uma senha gerada
    E o cliente do pedido deve ser nulo

Cenário: Criar um pedido com múltiplos itens do mesmo produto
    Dado que eu tenho um token de atendimento válido
    E que eu tenho um cliente identificado
    E que eu tenho os seguintes itens no pedido:
        | ProdutoId                              | Quantidade | PrecoUnitario | DescontoUnitario |
        | 33333333-3333-3333-3333-333333333333   | 5          | 10.00         | 1.00             |
    Quando eu criar o pedido
    Então o pedido deve ser criado com sucesso
    E o pedido deve conter 1 itens
    E o total de itens no pedido deve ser 5

Cenário: Criar um pedido com desconto
    Dado que eu tenho um token de atendimento válido
    E que eu tenho um cliente identificado
    E que o pedido tem subtotal de 100.00
    E que o pedido tem desconto de 10.00
    E que o pedido tem total de 90.00
    E que eu tenho os seguintes itens no pedido:
        | ProdutoId                              | Quantidade | PrecoUnitario | DescontoUnitario |
        | 44444444-4444-4444-4444-444444444444   | 2          | 50.00         | 5.00             |
    Quando eu criar o pedido
    Então o pedido deve ser criado com sucesso
    E o subtotal do pedido deve ser 100.00
    E o desconto do pedido deve ser 10.00
    E o total do pedido deve ser 90.00
