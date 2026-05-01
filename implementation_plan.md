# План: Создание своей игры на основе механик Maktala

## Что такое Maktala (анализ кода)

Maktala — это **2D авто-боёвка / idle roguelite** (жанр как Vampire Survivors):
- Игрок стоит в центре, враги автоматически спавнятся вокруг
- Скиллы срабатывают автоматически, башни помогают
- После гибели — метапрогрессия через дерево улучшений
- Несколько валют, предметы с редкостями, гемы

---

## Что мы будем делать

Создаём **СВОЮ** игру с теми же **механиками**, но:
- ✅ Полностью свой код (C# + Unity)
- ✅ Своё название, персонажи, мир
- ✅ Свои ассеты (бесплатные с itch.io / kenney.nl)
- ✅ Своя история и сеттинг
- ❌ Никаких файлов, кода, текстур из Maktala

---

## Предлагаемый сеттинг (пример)

**"AstroDefend"** — космический авто-шутер:
- Ты пилот на орбите, волны инопланетян атакуют со всех сторон
- Корабль стреляет автоматически, размещаешь турели, собираешь кристаллы
- Между запусками — апгрейды в командном центре

> [!IMPORTANT]
> Ты можешь выбрать **любой другой сеттинг**: фэнтези, постапокалипсис, подводный мир, и т.д. Скажи — и подстроим план.

---

## Архитектура игры (Unity, C# с нуля)

### Система 1: GameLoop / RunManager
```
RunManager
├── StartRun()      — начать новый забег
├── EndRun()        — завершить (смерть/победа)
├── isRunStarted    — флаг активности
└── OnRunEnd event  — событие для сохранения прогресса
```

### Система 2: PlayerManager
```
PlayerManager
├── Stats: HP, Speed, Damage, AttackSpeed, Range
├── OnDeath()
├── TakeDamage(float amount)
└── Currencies: Gold, RunPoints, MetaPoints
```

### Система 3: EnemiesManager (спавн врагов)
```
EnemiesManager
├── SpawnWave(int level)       — волна врагов
├── SmartSpawn()               — спавн вне видимости
├── AliveEnemies Dictionary    — все живые враги
├── EnemyTypes: Normal, Elite, Boss, Shiny (редкий)
└── DropLoot(EnemySelfer)      — дроп предметов/валюты
```

### Система 4: SkillsManager (авто-скиллы)
```
SkillsManager
├── List<SkillInfo> EquippedSkills
├── TriggerSkill(SkillInfo)     — выстрел/эффект
├── SkillCooldowns[]            — таймеры перезарядки
└── SkillTags: Projectile, AoE, Chain, Buff, Debuff
```

### Система 5: ProjectilesManager
```
ProjectilesManager
├── ObjectPooler               — пул снарядов
├── FireProjectile(dir, stats)
├── ProjectileTypes: Single, Multi, Orbit, Homing
└── OnHitEnemy(ProjectileInfo, EnemySelfer)
```

### Система 6: TowersManager (башни)
```
TowersManager
├── SpawnTower(TowerInfo, position)
├── TowerTypes: AoE, Circle, Gold, Poison
└── TowerChances Dictionary     — вероятности нахождения
```

### Система 7: ItemsManager / InventoryManager
```
InventoryManager
├── Items[6 слотов]
├── Rarity: Normal, Rare, Epic, Legendary
├── AutoSellNormalItems bool
└── ApplyItemStats(PlayerStats)
```

### Система 8: GemsManager (гемы)
```
GemsManager
├── Gems[4 слота]
├── GemLevel (1-20)
├── Reroll() — сменить стат гема
└── GemStats: DamageBonus, SpeedBonus, etc.
```

### Система 9: MetaTree (дерево между запусками)
```
TreeManager
├── TreeNodes[]          — узлы дерева
├── UnlockNode(node)     — трата MetaPoints
├── NodeConditions       — требования разблокировки
└── ApplyAllUnlocked()   — применить все апгрейды
```

### Система 10: PlayerData / SaveLoad
```
PlayerData (Singleton)
├── PlayerLevel, PlayerExp
├── AllCurrencies
├── UnlockedSkills, UnlockedSystems
├── AzrarLevels (аналог прокачки NPC)
└── SaveLoadManager (JSON сохранение)
```

---

## Предлагаемые изменения (100% оригинальный код)

### Фаза 1 — Ядро (2-3 дня)
- [ ] Новый Unity проект
- [ ] `GameManager` — точка входа, синглтон
- [ ] `RunManager` — игровой цикл
- [ ] `PlayerController` — движение, статы, смерть
- [ ] `EnemySpawner` — волны врагов вокруг игрока
- [ ] `EnemyAI` — движение к игроку + атака

### Фаза 2 — Боёвка (2-3 дня)
- [ ] `SkillSystem` — авто-скиллы с кулдаунами
- [ ] `ProjectilePool` — пул снарядов (ObjectPooler)
- [ ] `DamageSystem` — расчёт урона, крит, дебаффы
- [ ] `TowerSystem` — башни на карте

### Фаза 3 — Прогрессия (2-3 дня)
- [ ] `InventorySystem` — предметы, редкости
- [ ] `GemSystem` — гемы с уровнями
- [ ] `CurrencySystem` — золото, очки
- [ ] `LootDropper` — дроп с врагов

### Фаза 4 — Метапрогрессия (2-3 дня)
- [ ] `SkillTree` — дерево улучшений между запусками
- [ ] `SaveLoadSystem` — сохранение в JSON
- [ ] `AchievementsSystem` — достижения
- [ ] `MainMenu` + `RunEndScreen`

### Фаза 5 — Полировка
- [ ] UI (HP бар, DPS счётчик, таймер)
- [ ] Визуальные эффекты (частицы, шейки экрана)
- [ ] Звук и музыка
- [ ] Тестирование баланса

---

## Технологии
| Инструмент | Версия |
|---|---|
| Unity | 2022 LTS или 6000.x |
| Язык | C# |
| Анимации | Spine / Unity Animator |
| Физика | Unity 2D Physics |
| Сохранение | JSON через Newtonsoft.Json |
| Tweening | DOTween (бесплатный) |
| UI | TextMeshPro + Unity UI |

---

## Бесплатные ассеты
- 🎨 [Kenney.nl](https://kenney.nl/assets) — тысячи бесплатных спрайтов
- 🎮 [itch.io free assets](https://itch.io/game-assets/free) — тайлсеты, персонажи
- 🔊 [freesound.org](https://freesound.org) — звуки
- 🎵 [opengameart.org](https://opengameart.org) — музыка

---

## Open Questions

> [!IMPORTANT]
> **Вопрос 1**: Какой сеттинг тебе нравится?
> - 🚀 Космос (AstroDefend)
> - 🧙 Фэнтези / магия
> - 🧟 Зомби-апокалипсис
> - 🌊 Подводный мир
> - 💀 Тёмное фэнтези (как Maktala)
> - Другой?

> [!IMPORTANT]  
> **Вопрос 2**: С чего начинаем?
> - Хочешь чтобы я **сразу написал весь код** поэтапно?
> - Или сначала покажу **прототип одной механики** (например спавн врагов)?

> [!IMPORTANT]
> **Вопрос 3**: Unity уже установлен?
