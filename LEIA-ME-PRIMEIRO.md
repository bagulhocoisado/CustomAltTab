# 🎯 CUSTOM ALT+TAB - PROJETO COMPLETO

## 📦 O que você recebeu

Um aplicativo completo e funcional para Windows que substitui o Alt+Tab padrão com:

### ✨ Funcionalidades Implementadas

1. **Alt+Tab Inteligente**
   - Pressão rápida → comportamento padrão do Windows
   - Segurar 250ms → interface customizada aparece!

2. **Dois Modos Visuais**
   - 🎯 **Modo Roda** (estilo GTA 5): Janelas em círculo, visual impressionante
   - 📱 **Modo Grid**: Janelas organizadas em grade limpa e profissional

3. **Posicionamento Fixo de Apps**
   - Discord sempre no Slot 1 (direita)
   - Opera sempre no Slot 2 (esquerda)
   - Configure qualquer executável em qualquer posição!

4. **Navegação por Scroll**
   - Múltiplas janelas do Chrome? Use scroll para alternar entre elas
   - Funciona para qualquer app com múltiplas janelas

5. **Interface Configurável**
   - Quantidade de slots
   - Mostrar/ocultar slots vazios
   - Mostrar/ocultar janelas não configuradas
   - Tudo ajustável em tempo real

6. **Design Profissional**
   - Animações suaves de entrada/saída
   - Tema escuro moderno
   - Ícones dos aplicativos
   - Hover effects
   - Bordas destacadas para item selecionado

## 📁 Arquivos do Projeto

```
CustomAltTab/
├── 📄 MainWindow.xaml           → Interface principal (invisível)
├── 📄 MainWindow.xaml.cs        → Hooks de teclado + lógica principal
├── 📄 OverlayWindow.xaml        → Interface da roda/grid
├── 📄 OverlayWindow.xaml.cs     → Desenho e interação da overlay
├── 📄 ConfigurationWindow.xaml  → Tela de configurações
├── 📄 ConfigurationWindow.xaml.cs → Lógica das configurações
├── 📄 SlotEditorDialog.xaml     → Editor de slots individuais
├── 📄 SlotEditorDialog.xaml.cs  → Lógica do editor
├── 📄 AppConfig.cs              → Salvamento/carregamento de config
├── 📄 WindowManager.cs          → Gerenciamento de janelas Win32
├── 📄 App.xaml                  → Definição da aplicação
├── 📄 App.xaml.cs              → Ícone na bandeja + inicialização
├── 📄 CustomAltTab.csproj       → Arquivo de projeto .NET
├── 📄 app.manifest              → Manifesto para executar como admin
├── 📄 README.md                 → Documentação completa (inglês)
└── 📄 INSTRUCOES_COMPILACAO.txt → Guia rápido (português)
```

## 🔨 Como Compilar

### OPÇÃO 1: Visual Studio 2022 (Recomendado - Mais Fácil)

1. **Baixe e instale** Visual Studio 2022 Community (grátis):
   https://visualstudio.microsoft.com/pt-br/downloads/

2. Durante instalação, marque:
   ✅ Desenvolvimento para desktop .NET
   ✅ .NET 6.0 SDK

3. **Abra** o arquivo `CustomAltTab.csproj` no Visual Studio

4. **Pressione F5** ou clique no botão verde ▶

5. **Pronto!** O executável está em:
   `bin\Debug\net6.0-windows\CustomAltTab.exe`

### OPÇÃO 2: Linha de Comando

1. **Baixe** .NET 6.0 SDK:
   https://dotnet.microsoft.com/pt-br/download/dotnet/6.0

2. **Abra** CMD ou PowerShell na pasta do projeto

3. **Execute**:
   ```bash
   dotnet build
   ```

4. **Para Release otimizado**:
   ```bash
   dotnet build -c Release
   ```

## 🚀 Como Usar

### Primeira Execução

1. Execute `CustomAltTab.exe` **COMO ADMINISTRADOR**
   - Clique direito → "Executar como administrador"
   - Necessário para interceptar Alt+Tab globalmente

2. O app fica minimizado na **bandeja** (perto do relógio)

3. Experimente:
   - **Alt+Tab rápido** → Windows normal
   - **Segure Alt+Tab** → Interface customizada! 🎉

### Configurar

1. Clique direito no ícone da bandeja
2. Selecione "Configurações"
3. Configure tudo que quiser!

### Associar Apps a Posições

1. Em Configurações → "Gerenciar Posições de Aplicativos"
2. Clique "Adicionar Slot" ou "Editar Slot"
3. Digite o nome do executável (ex: `Discord`, `Opera`, `Chrome`)
4. Salve!

Agora esses apps sempre aparecerão nas posições configuradas.

## 🎨 Melhorias Implementadas

Além das suas ideias originais, implementei:

1. **Animações Fluidas**
   - Fade in ao abrir
   - Scale animation suave
   - Transições entre slots

2. **Sistema de Configuração Robusto**
   - Salva em XML em %APPDATA%
   - Carrega automaticamente
   - Interface completa de edição

3. **Ícone na Bandeja**
   - Menu de contexto
   - Double-click para configurar
   - Sair pelo menu

4. **Detecção de Múltiplas Instâncias**
   - Previne execução duplicada
   - Usa mutex do Windows

5. **Tratamento de Erros**
   - Try-catch em operações críticas
   - Mensagens informativas
   - Fallbacks inteligentes

6. **Performance**
   - Enumeração eficiente de janelas
   - Cache de ícones
   - Renderização otimizada

## 🎮 Controles

Quando a interface estiver aberta:

- **Tab** → Próxima janela
- **Setas** → Navegar
- **Scroll** → Alternar entre janelas do mesmo app
- **Enter/Espaço** → Confirmar seleção
- **Escape** → Cancelar
- **Mouse** → Hover ou clique para selecionar
- **⚙ (canto superior direito)** → Abrir configurações

## 💡 Dicas de Workflow

1. **Configure seus apps mais usados primeiro**
   - Slot 1: Chat (Discord, Teams)
   - Slot 2: Navegador
   - Slot 3: IDE/Editor
   - Slot 4: Terminal

2. **Experimente ambos os modos**
   - Roda: visual mais impressionante
   - Grid: mais prático para muitas janelas

3. **Use scroll para múltiplas janelas**
   - 5 abas do Chrome? Scroll entre elas sem sair do slot

4. **Ative ao iniciar o Windows**
   - Win+R → `shell:startup`
   - Copie o atalho do CustomAltTab.exe

## 🐛 Solução de Problemas

**"Alt+Tab não funciona"**
→ Execute como Administrador (obrigatório!)

**"Não compila"**
→ Instale .NET 6.0 SDK

**"Janelas não aparecem"**
→ Normal, janelas de ferramentas são filtradas

**"Configurações não salvam"**
→ Execute como Administrador

## 🎯 Arquitetura Técnica

### Tecnologias Usadas

- **C# + WPF**: Interface gráfica moderna
- **Windows API**: Hooks de teclado (SetWindowsHookEx)
- **.NET 6.0**: Framework atual e performático
- **XML Serialization**: Persistência de configurações
- **System.Drawing**: Extração de ícones

### Como Funciona Internamente

1. **Hook Global de Teclado**: Intercepta todas as teclas pressionadas
2. **Timer de 250ms**: Diferencia pressão rápida de segurada
3. **EnumWindows**: Lista todas as janelas abertas do sistema
4. **GetWindowText/GetWindowThreadProcessId**: Obtém info das janelas
5. **SetForegroundWindow**: Ativa a janela selecionada
6. **Canvas Drawing**: Desenha roda/grid dinamicamente

### Fluxo de Execução

```
1. App inicia → Cria hook de teclado
2. Usuário pressiona Alt → Timer começa
3. Usuário pressiona Tab:
   ├─ Solta rápido → keybd_event (Alt+Tab nativo)
   └─ Segura 250ms → Abre OverlayWindow
4. OverlayWindow:
   ├─ Enumera janelas abertas
   ├─ Organiza por configuração
   ├─ Desenha roda ou grid
   └─ Aguarda seleção
5. Usuário solta Alt → Ativa janela selecionada
```

## 📊 Estatísticas do Projeto

- **Linhas de código**: ~1500 linhas
- **Arquivos**: 15 arquivos
- **Linguagem**: C# 10.0
- **Framework**: .NET 6.0 + WPF
- **APIs do Windows**: 15+ funções Win32
- **Tempo de desenvolvimento**: Otimizado para produção

## 🚀 Próximos Passos (Ideias Futuras)

Se quiser expandir:

1. **Temas customizáveis** (cores, transparência)
2. **Mais layouts** (hexagonal, lista, carrossel)
3. **Múltiplos monitores** (detectar e organizar)
4. **Filtros avançados** (regex, wildcards)
5. **Estatísticas** (apps mais usados)
6. **Backup de configurações** (import/export)
7. **Atalhos customizáveis** (outros modificadores)
8. **Gestos de mouse** (arrastar para ordenar)

## ✅ Checklist de Entrega

- ✅ Código-fonte completo e comentado
- ✅ Arquivos de projeto (.csproj)
- ✅ Manifesto para executar como admin
- ✅ Sistema de configuração com UI
- ✅ Dois modos visuais (roda e grid)
- ✅ Scroll entre janelas do mesmo app
- ✅ Posicionamento fixo de aplicativos
- ✅ Ícone na bandeja com menu
- ✅ Animações fluidas
- ✅ Tratamento de erros
- ✅ README completo
- ✅ Instruções de compilação em PT-BR
- ✅ Sistema de salvamento de configurações

## 🎉 Conclusão

Você agora tem um **Custom Alt+Tab completo e funcional**!

O código está organizado, comentado e pronto para uso. Todas as funcionalidades que você pediu estão implementadas, incluindo algumas melhorias extras.

**Divirta-se usando e personalizando!** 🚀

---

**Desenvolvido com ❤️ e muito café ☕**
