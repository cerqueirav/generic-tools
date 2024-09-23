# GenericToolsAPI

<a id="about"></a>

## Sobre

O **GenericToolsAPI** é um conjunto de ferramentas projetadas em C# para facilitar o desenvolvimento de aplicações em diversas áreas, oferecendo funcionalidades como geolocalização, conversão de formatos e notificações.

<a id="features"></a>

## Funcionalidades

### Funcionalidades Disponíveis

- **Conversor**
  - `POST /Conversor/excel-to-sql`

- **Localização**
  - `POST /Localizacao/geolocalizacao`
  - `POST /Localizacao/geolocalizacao-reversa`
  - `POST /Localizacao/limites-cidade`
  - `POST /Localizacao/buscar-poi`

- **Notificação**
  - `POST /Notificacao/email/enviar`
  - `POST /Notificacao/sms/enviar`
  - `POST /Notificacao/whatsapp/enviar`

- **QRCode**
  - `GET /QRCode/gerar-qrcode`

#### Schemas
- `CoordenadasRequest`
- `EmailRequest`
- `EnderecoRequest`
- `LimiteCidadeRequest`
- `PontoInteresseRequest`
- `SmsRequest`
- `WhatsAppRequest`

<a id="adjustments"></a>

## Ajustes e Melhorias

O projeto está em constante desenvolvimento, e as próximas atualizações estarão focadas nas seguintes tarefas:

### Melhorias em Funcionalidades 🟠
- Implementar validações adicionais para os inputs de localização. 🟠
- Melhorar a documentação do Swagger UI. 🟠

##### Legenda
- 🟢 = `Feito`
- 🟠 = `Fazendo`
- 🔴 = `Pendente/Falta`

<a id="technologies-used"></a>

## Tecnologias Utilizadas

Esse projeto foi desenvolvido utilizando as seguintes tecnologias e pacotes:

- [.NET Core](https://dotnet.microsoft.com/)
- [Swagger](https://swagger.io/)

<a id="how-to-use"></a>

## Como Clonar e Importar

- Faça um fork do projeto.
- Abra o terminal do Visual Studio Code.
- Digite (troque `cerqueirav` pelo nome do seu usuário): 
  ```bash
  git clone https://github.com/cerqueirav/GenericToolsAPI
