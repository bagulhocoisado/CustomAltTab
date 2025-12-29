# 🎯 Custom Alt+Tab

Um substituto elegante e personalizável para o Alt+Tab do Windows com interface estilo GTA 5 ou Grid!

## ✨ Funcionalidades

- **Alt+Tab Rápido**: Solta rapidamente = comportamento padrão do Windows
- **Alt+Tab Segurado**: Segura por 250ms = abre interface customizada
- **Dois Modos Visuais**:
  - 🎯 **Roda** (estilo GTA 5): Janelas organizadas em círculo
  - 📱 **Grid**: Janelas em grade organizada
- **Posições Fixas**: Associe executáveis a posições específicas (Discord sempre à direita, Opera à esquerda)
- **Ações Especiais**:
  - ⬇️ **Minimizar Atual**: Minimiza apenas a janela ativa (perfeito para jogos fullscreen!)
  - ❌ **Cancelar**: Fecha a roda sem fazer nada
- **Scroll entre Janelas**: Use scroll do mouse para alternar entre múltiplas janelas do mesmo app
- **Visuais Impressionantes**:
  - 🌟 Blur backdrop cinematográfico
  - ✨ Efeitos de glow e hover animados
  - 🎨 Gradientes radiais elegantes
  - 💫 Animações fluidas com 60 FPS
  - 🏷️ Badges de contagem para apps com múltiplas janelas
- **Totalmente Configurável**: Quantidade de slots, slots vazios, janelas não configuradas, etc.

## 🎮 Como Usar

### Atalhos Básicos

1. **Alt+Tab rápido** → Volta para última janela (comportamento padrão)
2. **Segure Alt+Tab** → Abre interface customizada
3. **Tab** → Navega entre janelas
4. **Setas** → Navega entre janelas
5. **Scroll** → Alterna entre múltiplas janelas do mesmo aplicativo
6. **Enter/Espaço** → Seleciona janela
7. **Escape** → Cancela
8. **Mouse** → Hover ou clique para selecionar

### Configuração

1. Clique com botão direito no ícone da bandeja do sistema
2. Selecione "Configurações"
3. Configure:
   - Modo de exibição (Roda ou Grid)
   - Quantidade mínima de slots
   - Mostrar slots vazios
   - Mostrar janelas não configuradas
   - Adicionar/editar/remover slots específicos

### Associar Aplicativos a Posições

1. Abra Configurações
2. Na seção "Gerenciar Posições de Aplicativos"
3. Clique em "Adicionar Slot" ou selecione um slot existente e clique "Editar"
4. **Escolha o tipo de slot:**
   - 📱 **Aplicativo**: Digite o nome do executável (ex: `Discord`, `Opera`, `Chrome`)
   - ⬇️ **Minimizar Atual**: Cria um slot que minimiza a janela ativa
   - ❌ **Cancelar**: Cria um slot que fecha a roda sem fazer nada
5. Para aplicativos: Digite o nome ou selecione de processos em execução
6. Salve

**Exemplo de Configuração:**
- Slot 1: Cancelar (topo da roda)
- Slot 2: Discord
- Slot 3: Opera
- Slot 4: vscode
- Slot 5: Minimizar Atual (baixo da roda)

**Caso de uso - Minimizar jogos:**
```
Você está jogando em fullscreen
→ Segura Alt+Tab
→ Move mouse para baixo (slot Minimizar)
→ Solta Alt
→ Jogo minimiza sem alternar para outra janela!
```

## 📦 Compilação

### Requisitos

1. **Visual Studio 2022** (ou superior) com:
   - Desenvolvimento para desktop .NET
   - .NET 6.0 SDK ou superior

   **OU**

2. **.NET 6.0 SDK** standalone:
   - Baixe em: https://dotnet.microsoft.com/download/dotnet/6.0

### Opção 1: Compilar com Visual Studio

1. Abra `CustomAltTab.csproj` no Visual Studio
2. Pressione `F5` para compilar e executar
3. Ou vá em `Build > Build Solution`
4. O executável estará em `bin\Debug\net6.0-windows\CustomAltTab.exe`

### Opção 2: Compilar via Linha de Comando

```bash
# Navegue até a pasta do projeto
cd caminho\para\CustomAltTab

# Compile o projeto
dotnet build

# Para compilar em modo Release (otimizado)
dotnet build -c Release

# Para publicar como executável único
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
```

O executável estará em:
- Debug: `bin\Debug\net6.0-windows\CustomAltTab.exe`
- Release: `bin\Release\net6.0-windows\CustomAltTab.exe`
- Publish: `bin\Release\net6.0-windows\win-x64\publish\CustomAltTab.exe`

### 🚀 Compilação Automática com GitHub Actions

Este projeto inclui um workflow do GitHub Actions que **compila automaticamente** quando você cria uma release!

**Como usar:**
1. Faça push do código para o GitHub
2. Crie uma tag de versão: `git tag v1.0.0 && git push origin v1.0.0`
3. O GitHub compila automaticamente e cria uma release
4. O executável fica disponível em "Releases" para download

**Vantagens:**
- ✅ Não precisa ter Visual Studio instalado
- ✅ Compilação consistente e confiável
- ✅ Releases profissionais e organizadas
- ✅ Totalmente gratuito para repositórios públicos

**Veja o guia completo:** `GITHUB_ACTIONS_GUIA.md`

## 🚀 Instalação

### Primeira Execução

1. Execute `CustomAltTab.exe` **como Administrador** (necessário para hooks de teclado globais)
2. O aplicativo ficará minimizado na bandeja do sistema
3. Configure suas preferências clicando com botão direito no ícone

### Executar Automaticamente ao Iniciar o Windows

**Opção 1: Criar Atalho na Pasta Inicializar**

1. Pressione `Win + R`
2. Digite: `shell:startup`
3. Copie o atalho do `CustomAltTab.exe` para esta pasta

**Opção 2: Task Scheduler (Recomendado para executar como Admin)**

1. Abra "Agendador de Tarefas"
2. Crie Nova Tarefa:
   - Nome: Custom Alt+Tab
   - Executar com privilégios mais altos: ✓
   - Gatilho: Ao fazer logon
   - Ação: Iniciar `CustomAltTab.exe`

## 🎨 Design e Arquitetura

### Estrutura do Projeto

```
CustomAltTab/
├── MainWindow.xaml/cs       # Janela invisível principal (hooks)
├── OverlayWindow.xaml/cs    # Interface de seleção (roda/grid)
├── ConfigurationWindow.xaml/cs  # Janela de configurações
├── SlotEditorDialog.xaml/cs # Editor de slots individuais
├── AppConfig.cs             # Gerenciamento de configurações
├── WindowManager.cs         # Gerenciamento de janelas do Windows
├── App.xaml/cs             # Aplicação e ícone na bandeja
└── README.md               # Este arquivo
```

### Tecnologias Utilizadas

- **C# + WPF**: Interface gráfica
- **Windows API**: Hooks de teclado e gerenciamento de janelas
- **XML Serialization**: Armazenamento de configurações
- **.NET 6.0**: Framework moderno e performático

### Como Funciona

1. **Hooks Globais**: Intercepta Alt+Tab usando `SetWindowsHookEx`
2. **Timer de Segurar**: 250ms para distinguir entre pressão rápida e segurada
3. **Enumeração de Janelas**: Usa `EnumWindows` da Windows API
4. **Animações**: WPF animations para transições suaves
5. **Configuração Persistente**: XML em `%APPDATA%\CustomAltTab\config.xml`

## 🐛 Solução de Problemas

### "O aplicativo não está interceptando Alt+Tab"

- **Solução**: Execute como Administrador
- Hooks de teclado globais requerem privilégios elevados

### "Algumas janelas não aparecem"

- Janelas de ferramentas e popups são intencionalmente filtradas
- Verifique se a janela tem título e é visível

### "Configurações não estão sendo salvas"

- Verifique permissões de escrita em `%APPDATA%\CustomAltTab\`
- Execute como Administrador

### "Alt+Tab padrão ainda funciona junto"

- Isso é esperado quando você solta rapidamente
- Para Alt+Tab customizado, segure por pelo menos 250ms

## 🎯 Roadmap Futuro

- [ ] Temas personalizáveis
- [ ] Mais layouts (hexagonal, lista)
- [ ] Integração com múltiplos monitores
- [ ] Filtros avançados de janelas
- [ ] Estatísticas de uso
- [ ] Atalhos customizáveis
- [ ] Suporte a gestos de mouse

## 📝 Licença

Este projeto é livre para uso pessoal e comercial.

## 🤝 Contribuições

Sinta-se livre para fazer fork, melhorar e enviar pull requests!

## 💡 Dicas

1. **Performance**: Em modo Grid, limite o número de janelas para melhor performance
2. **Múltiplas Janelas**: Use scroll para alternar entre várias janelas do mesmo app
3. **Organização**: Configure seus apps mais usados nos primeiros slots
4. **Workflow**: Experimente ambos os modos (Roda e Grid) para ver qual prefere

---

**Desenvolvido com ❤️ para melhorar sua produtividade no Windows!**
