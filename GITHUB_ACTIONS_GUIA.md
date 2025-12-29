# 🚀 GUIA COMPLETO: GitHub Actions & Releases

## ❓ Como Funciona?

Quando você vê a aba "Releases" em projetos do GitHub, existem duas formas de compilar:

### OPÇÃO 1: Compilação Automática no GitHub (Recomendada) ✨

O GitHub compila automaticamente usando **GitHub Actions** (CI/CD gratuito).
Você não precisa ter Visual Studio instalado!

**Como funciona:**
1. Você faz push do código para o GitHub
2. O GitHub detecta o arquivo `.github/workflows/build.yml`
3. Um servidor do GitHub compila o projeto automaticamente
4. O executável é criado e disponibilizado para download

### OPÇÃO 2: Compilação Manual

Você compila no seu PC e depois faz upload do executável.
(Menos prático e profissional)

---

## 🎯 CONFIGURANDO GITHUB ACTIONS (Automático)

### Passo 1: Subir o Projeto para o GitHub

```bash
# No terminal, dentro da pasta CustomAltTab:

# 1. Inicializar repositório Git
git init

# 2. Adicionar todos os arquivos
git add .

# 3. Fazer primeiro commit
git commit -m "Initial commit - Custom Alt+Tab"

# 4. Criar repositório no GitHub
# Vá para https://github.com/new
# Crie um repositório chamado "CustomAltTab"

# 5. Conectar ao repositório remoto
git remote add origin https://github.com/SEU_USUARIO/CustomAltTab.git

# 6. Enviar código
git branch -M main
git push -u origin main
```

### Passo 2: Criar uma Release

Agora que o código está no GitHub, você tem duas opções:

#### OPÇÃO A: Criar Tag via Terminal (Recomendado)

```bash
# Criar uma tag de versão
git tag v1.0.0

# Enviar a tag para o GitHub
git push origin v1.0.0
```

#### OPÇÃO B: Criar Release pelo Site

1. Vá para seu repositório no GitHub
2. Clique em "Releases" (lado direito)
3. Clique em "Create a new release"
4. Em "Choose a tag", digite: `v1.0.0` e clique "Create new tag"
5. Preencha:
   - **Release title**: `v1.0.0 - Primeira Versão`
   - **Description**: Descreva as funcionalidades
6. Clique em "Publish release"

### Passo 3: GitHub Actions Compila Automaticamente! 🎉

1. Vá para a aba "Actions" no seu repositório
2. Você verá o workflow "Build and Release" rodando
3. Aguarde ~5 minutos (compilação no servidor do GitHub)
4. Quando terminar, volte para "Releases"
5. Seu executável estará lá! 🎊

**O arquivo gerado será:**
`CustomAltTab-Windows-x64.zip`

---

## 📥 Usar o Executável Compilado

Qualquer pessoa pode agora:
1. Ir em "Releases" no seu GitHub
2. Baixar `CustomAltTab-Windows-x64.zip`
3. Extrair e executar `CustomAltTab.exe`
4. **Sem precisar compilar nada!**

---

## 🔄 Workflow Completo

```
1. Você edita o código localmente
2. git add .
3. git commit -m "Adicionei nova funcionalidade"
4. git push
5. git tag v1.0.1
6. git push origin v1.0.1
7. GitHub Actions compila automaticamente
8. Nova release aparece em alguns minutos
```

---

## ⚙️ O Arquivo .github/workflows/build.yml

Este arquivo diz ao GitHub **como compilar** seu projeto:

```yaml
name: Build and Release

on:
  push:
    tags:
      - 'v*'  # Dispara quando você criar tag v1.0.0, v2.0.0, etc
  workflow_dispatch:  # Permite executar manualmente

jobs:
  build:
    runs-on: windows-latest  # Usa Windows no servidor do GitHub
    
    steps:
    - name: Baixar código
      uses: actions/checkout@v3
      
    - name: Instalar .NET 8.0
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
        
    - name: Compilar projeto
      run: dotnet build -c Release
      
    - name: Criar executável único
      run: dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
      
    - name: Criar ZIP
      run: Compress-Archive -Path ./publish/* -DestinationPath CustomAltTab-Windows-x64.zip
      
    - name: Fazer upload para Release
      uses: softprops/action-gh-release@v1
      with:
        files: CustomAltTab-Windows-x64.zip
```

---

## 🎨 Exemplo de Versionamento

```bash
# Primeira versão pública
git tag v1.0.0
git push origin v1.0.0

# Correção de bug
git tag v1.0.1
git push origin v1.0.1

# Nova funcionalidade
git tag v1.1.0
git push origin v1.1.0

# Grande atualização
git tag v2.0.0
git push origin v2.0.0
```

---

## 🆓 É Grátis?

**SIM!** GitHub Actions é **100% gratuito** para repositórios públicos!

- ✅ Ilimitado para projetos públicos
- ✅ 2.000 minutos/mês para projetos privados
- ✅ Compilação em Windows, Linux e macOS

---

## 🎯 Checklist Rápido

- [ ] Código no GitHub
- [ ] Arquivo `.github/workflows/build.yml` está presente
- [ ] Criar tag `v1.0.0`
- [ ] Push da tag
- [ ] Aguardar GitHub Actions compilar
- [ ] Verificar em "Releases"
- [ ] Compartilhar o link!

---

## 📱 Resultado Final

Depois de tudo configurado, você terá:

```
https://github.com/SEU_USUARIO/CustomAltTab/releases

Releases
├── v1.0.0
│   └── CustomAltTab-Windows-x64.zip (executável pronto)
├── v1.0.1
│   └── CustomAltTab-Windows-x64.zip
└── v2.0.0
    └── CustomAltTab-Windows-x64.zip
```

---

## 🐛 Solução de Problemas

**"GitHub Actions falhou"**
→ Vá em "Actions" e clique no workflow que falhou
→ Leia os logs para ver o erro
→ Geralmente é problema de sintaxe no código

**"Não aparece em Releases"**
→ Certifique-se de criar uma **tag** (não apenas commit)
→ A tag deve começar com 'v' (v1.0.0)

**"Quero recompilar sem criar nova tag"**
→ Vá em "Actions" > "Build and Release" > "Run workflow"

---

## 💡 Dicas Profissionais

1. **README bonito**: Adicione badges:
   ```markdown
   ![Build](https://github.com/SEU_USUARIO/CustomAltTab/workflows/Build%20and%20Release/badge.svg)
   ```

2. **Changelog**: Documente mudanças em cada versão

3. **Pre-releases**: Para versões beta:
   ```bash
   git tag v1.0.0-beta
   ```

4. **Notificações**: GitHub notifica seguidores a cada release

---

## 🎉 Pronto!

Agora você tem um projeto profissional com:
- ✅ Compilação automática
- ✅ Releases organizadas
- ✅ Download fácil para usuários
- ✅ Versionamento adequado
- ✅ Zero custo

**Seus usuários nunca mais precisarão ter Visual Studio instalado!**

---

**Links Úteis:**
- [GitHub Actions Docs](https://docs.github.com/actions)
- [Criar Releases](https://docs.github.com/repositories/releasing-projects-on-github)
- [Semantic Versioning](https://semver.org/)
