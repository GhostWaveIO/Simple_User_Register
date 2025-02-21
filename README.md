# SUR - Simple User Register

SUR (Simple User Register) é um sistema simples e de código aberto para cadastro de usuários, desenvolvido em .NET 8. Ele permite personalizar o formulário de cadastro, definindo tipos de dados, a posição dos campos e das linhas dentro dos grupos. O sistema é compatível com Windows e Linux.

## 🛠️ Configuração Inicial

Antes de iniciar, siga os passos abaixo:

1. **Renomeie o arquivo de configuração:**
   ```sh
   mv "appsettings - BK.json" "appsettings.json"
   ```
   Esse arquivo contém a string de conexão com o banco de dados.

2. **Substitua as imagens de marca** (logotipo, favicon, etc.)
   - Local das imagens: `/StaticFiles/wwwroot/Imagens`

3. **Execute o sistema ao menos uma vez** para gerar o arquivo de configuração:
   - Local do arquivo: `/StaticFiles/internal/settings/general.json`
   - Configure esse arquivo antes de prosseguir.

4. **Execute o migrations na pasta do projeto:**
   ```sh
   dotnet ef migrations add first
   dotnet ef database update
   ```

5. **Crie a estrutura de cadastro e o usuário administrador:**
   - Acesse a URL:
     ```
     http://localhost:xxx/Login/CorrecaoGeral
     ```
   - Esse comando deve ser executado apenas uma vez.

## 👤 Login Padrão

- **Usuário:** Configurado no arquivo `/StaticFiles/internal/settings/general.json`
- **Senha inicial:** `admin12345`

## 📝 Estrutura do Formulário

O formulário segue uma estrutura hierárquica:

1. **Grupos** - Seções principais do formulário.
2. **Linhas** - Organizam os campos horizontalmente dentro de um grupo.
3. **Campos** - Cada campo ocupa espaço dentro da linha (de 1 a 12 colunas, similar ao Bootstrap).

Os campos podem ser organizados e reposicionados livremente, mas os campos genéricos não podem ser removidos.

### 🏛️ Tipos de Campos

- **Inputs (preenchidos pelo usuário):**
  - CheckBox
  - Documento
  - Email
  - Imagem
  - Número
  - Número Monetário
  - Radio Button
  - Select
  - Text 250
  - Text Longo
- **Views (exibição de conteúdo):**
  - Html

## 🌐 Homes Personalizáveis

- Possibilidade de criar até 20 Homes personalizadas com HTML e JavaScript.
- Permissão para definir quais usuários podem ver cada Home.
- Exemplo de permissão:
  - Usuário 1 (**admin**) -> Home 1 e 2.
  - Usuário 2 (**membro**) -> Home 2 e 3.

## 🔎 Pesquisa de Usuários
O sistema conta com um local dedicado para pesquisar usuários cadastrados.

## 🌟 Capturas de Tela

1. **Tela de Login**
   
2. **Tela Home** (Exibe as homes configuradas para cada usuário)
   
3. **Editor de Formulário** (Personalização de grupos, linhas e campos)
   
4. **Gerenciamento de Permissões**

## 💪 Contribuição
Sinta-se à vontade para contribuir com melhorias!

1. **Clone o repositório:**
   ```sh
   git clone https://github.com/GhostWaveIO/Simple_User_Register.git
   ```
2. **Crie uma branch:**
   ```sh
   git checkout -b minha-feature
   ```
3. **Realize suas alterações e commit:**
   ```sh
   git commit -m "Minha contribuição"
   ```
4. **Envie as alterações:**
   ```sh
   git push origin minha-feature
   ```
5. **Abra um Pull Request!**

---
Feito com ❤️ por [GhostWaveIO](https://github.com/GhostWaveIO).

