# Selenium WebDriver - Automações do YouTube

Aplicativo de console em C# (.NET 9) que usa o Selenium para automatizar o Google Chrome.

O objetivo principal é deixar um PC (por exemplo, na recepção) passando lives do YouTube em tela inteira, sem o chat aparecendo e sem ninguém precisar mexer.

## Requisitos

- .NET 9 SDK
- Google Chrome instalado via Flatpak (o caminho está em `chromeBinary`)
- Pacotes NuGet (já estão no `ConsoleApp1.csproj`): `Selenium.WebDriver` e `Selenium.WebDriver.ChromeDriver`

## Como rodar

```bash
dotnet run
```

Escolha uma opção no menu:

```
=== Automations ===
1 - Youtube Music
2 - Youtube
```

Para encerrar, aperte `Ctrl+C` ou feche a janela do Chrome.

## Perfil do Chrome

O Chrome (versão 136+) bloqueia automação no perfil padrão. Por isso o projeto usa um perfil separado em `~/.config/selenium-chrome-profile`.

Na primeira execução, faça login na sua conta do YouTube nesse perfil. A sessão fica salva para as próximas vezes.

## Estrutura

| Arquivo | O que faz |
| --- | --- |
| `Program.cs` | Menu inicial, chama a automação escolhida |
| `YtMusicAutomation.cs` | Abre uma música no YouTube Music e clica em play |
| `YtCloseChat.cs` | Abre uma live do YouTube, fecha o chat, coloca em tela inteira e monitora o chat |

## Lógica da automação do YouTube (`YtCloseChat.cs`)

1. **Abre o Chrome** com o perfil de automação e navega até a live.
2. **Fecha o chat:** o chat da live fica dentro de um `iframe` (`#chatframe`), então o Selenium entra nele (`SwitchTo().Frame`), procura o botão de fechar pelo XPath e clica.
3. **Sai do iframe** (`SwitchTo().DefaultContent()`), porque o player fica na página do vídeo e não dentro do chat.
4. **Entra na tela inteira** clicando no botão do player (`.ytp-fullscreen-button`).
5. **Tenta fechar o chat de novo**, porque ele pode voltar ao entrar na tela inteira.
6. **Fica monitorando** até o programa ser encerrado (veja abaixo).

## Como funciona o monitoramento

Quando uma live acaba, o YouTube vai sozinho para a próxima, e a próxima abre com o chat aberto. Para resolver isso, o programa fica num laço que roda a cada poucos segundos (intervalo definido em `shutdown.Wait(TimeSpan.FromSeconds(...))`).

Em cada volta do laço:

1. Confere se o navegador ainda está aberto. Se foi fechado, encerra o programa.
2. Chama `CloseChatIfOpen`, que:
   - procura o iframe do chat com `FindElements` (no plural). Diferente do `FindElement`, ele **não espera nem dá erro**: devolve uma lista, que vem vazia se não houver chat;
   - se a lista estiver vazia (`Count == 0`), não tem chat e ele sai sem fazer nada;
   - se tiver, entra no iframe e procura o botão de fechar do mesmo jeito;
   - se o botão existe (`Count > 0`) **e** está aparecendo na tela (`Displayed`), clica e escreve `Chat opened again, closed it.` no console;
   - no final, sempre sai do iframe, para a próxima checagem começar da página do vídeo.
3. Se a página estiver trocando de live bem na hora da checagem e der algum erro, o erro é ignorado e o programa tenta de novo na próxima volta.

Resumindo: em cada volta o programa pergunta "tem chat aberto?". Se tiver, fecha; se não, espera a próxima volta.

## Observações

- O botão de fechar o chat é encontrado por um **XPath absoluto** (`CloseChatXPath`). Se o YouTube mudar o layout do chat, esse XPath pode parar de funcionar. O programa não quebra, só deixa de fechar o chat, e aí é preciso atualizar o XPath.
- O monitoramento cuida só do chat. Se o YouTube sair da tela inteira na troca de live, o programa não volta sozinho para ela.
