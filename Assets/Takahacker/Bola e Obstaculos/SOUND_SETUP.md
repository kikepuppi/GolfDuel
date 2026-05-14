# Sistema de Sons Completo - Golf Duel

## ✅ Sons Implementados

### 1. **Som da Tacada** 🏌️

- Script: `GolfInput.cs` (modificado)
- Toca quando o jogador faz o swing

### 2. **Som do Buraco** ⛳

- Script: `HoleTrigger.cs` (modificado)
- Toca quando a bola entra no buraco

### 3. **Som de Colisão da Bola** 💥

- Script: `BallCollisionSound.cs`
- Som genérico quando a bola colide com algo

### 4. **Som de Seleção de Objeto** 🎯

- Script: `DragAndPlace.cs` (modificado)
- Sons específicos para cada objeto quando clicado:
  - 🐄 **Vaca (Cow)** - Som de mugir
  - 🦊 **Raposa (Fox)** - Som de ladrar/grito
  - 🌳 **Árvore (Tree)** - Som de folhas/galho
  - 🪨 **Rocha (Rock)** - Som de impacto/pedra
  - 🌬️ **Moinho (Windmill)** - Som de vento/engrenagem

### 5. **Som de Colisão de Objeto** 💫

- Script: `ObstacleCollisionSound.cs` (novo)
- Sons específicos para cada obstáculo quando a bola colide

---

## 📋 Configuração no Unity

### Passo 1: Criar SoundManager

1. Crie um GameObject vazio: `SoundManager`
2. Adicione o componente `SoundManager`
3. Configure TODOS estes campos no Inspector:

#### Som Geral

- **Shot Sound**: Som da tacada
- **Hole Sound**: Som do buraco
- **Collision Sound**: Som genérico de colisão

#### Sons de Seleção (5 campos)

- **Cow Select Sound**: Som quando clica em vaca
- **Fox Select Sound**: Som quando clica em raposa
- **Tree Select Sound**: Som quando clica em árvore
- **Rock Select Sound**: Som quando clica em rocha
- **Windmill Select Sound**: Som quando clica em moinho

#### Sons de Colisão (5 campos)

- **Cow Collision Sound**: Som quando bola colide com vaca
- **Fox Collision Sound**: Som quando bola colide com raposa
- **Tree Collision Sound**: Som quando bola colide com árvore
- **Rock Collision Sound**: Som quando bola colide com rocha
- **Windmill Collision Sound**: Som quando bola colide com moinho

### Passo 2: Configurar cada Obstáculo

**Para CADA prefab de obstáculo** (Cow, Fox, Tree, Rock, Windmill):

1. Abra o prefab (ou o GameObject instanciado)
2. Adicione o componente `ObstacleCollisionSound`
3. Configure o tipo no Inspector:
   - Cow → ObstacleType = "Cow"
   - Fox → ObstacleType = "Fox"
   - Tree → ObstacleType = "Tree"
   - Rock → ObstacleType = "Rock"
   - Windmill → ObstacleType = "Windmill"
4. Ajuste `Min Impact Velocity` se necessário (padrão: 1.5)

### Passo 3: Adicionar som à Bola

1. Selecione o GameObject da bola
2. Adicione o componente `BallCollisionSound`
3. Ajuste `Min Impact Velocity` (padrão: 2)

---

## 🎵 Recomendações de Sons

**Som da Tacada:**

- Som seco e curto (150-300ms)
- Tipo: Golpe de clube, "whoosh", ou efeito eletrônico

**Som do Buraco:**

- Som que indica sucesso (500-800ms)
- Tipo: Trifo, chime, ou efeito de conclusão

**Som de Colisão:**

- Sons curtos e variados (100-200ms)
- Tipo: Impacto suave, "bop", toque de tambor

---

## 🔧 Variáveis Editáveis

| Script                 | Variável          | Padrão | Descrição                                      |
| ---------------------- | ----------------- | ------ | ---------------------------------------------- |
| SoundManager           | shotVolume        | 0.7    | Volume da tacada (0-1)                         |
| SoundManager           | holeVolume        | 1.0    | Volume do buraco (0-1)                         |
| SoundManager           | collisionVolume   | 0.5    | Volume de colisão (0-1)                        |
| BallCollisionSound     | minImpactVelocity | 2.0    | Velocidade mínima para som de colisão          |
| BallCollisionSound     | collisionCooldown | 0.1    | Tempo mínimo entre sons de colisão (anti-spam) |
| SoundManager           | selectVolume      | 0.6    | Volume de seleção de objeto (0-1)              |
| ObstacleCollisionSound | minImpactVelocity | 1.5    | Velocidade mínima para obstáculo colidir       |
| ObstacleCollisionSound | collisionCooldown | 0.15   | Tempo entre sons de colisão de obstáculo       |

---

## 📝 Resumo dos Arquivos Criados/Modificados

### Criados:

- `SoundManager.cs` - Gerenciador centralizado
- `BallCollisionSound.cs` - Colisão genérica da bola
- `ObstacleCollisionSound.cs` - Colisão específica de obstáculos
- `SOUND_SETUP.md` - Este guia

### Modificados:

- `GolfInput.cs` - Som de tacada
- `HoleTrigger.cs` - Som do buraco
- `DragAndPlace.cs` - Sons de seleção de objetos

---

## 📝 Notas Técnicas

- O **SoundManager** usa `PlayOneShot()` para não interromper outros sons
- O **BallCollisionSound** tem um cooldown para evitar spam de sons
- Sons não tocam se:
  - O AudioClip não estiver configurado
  - A bola colidir com o buraco (evita sons duplicados)
  - A velocidade de colisão for menor que o mínimo

---

## ❓ Troubleshooting

**Sons não funcionam:**

- ✓ Certifique-se de que o SoundManager está na cena
- ✓ Verifique se os AudioClips estão atribuídos no Inspector
- ✓ Confirm que o volume do AudioSource não está em 0

**Colisões tocam som muito frequente:**

- Aumente o valor de `collisionCooldown` em BallCollisionSound

**Colisões não tocam som:**

- Aumentar `minImpactVelocity` em BallCollisionSound
- Verifique se a bola tem um Rigidbody2D
