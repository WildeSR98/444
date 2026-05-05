# Вымесы: Последний Свет

Тёмное-фэнтези авто-боёвка / idle-roguelite в духе Vampire Survivors. Игрок в центре, орды **вымесов** атакуют со всех сторон, скиллы срабатывают автоматически, башни помогают, после смерти — мета-прогрессия через дерево узлов.

> **Полностью оригинальный код.** Никакие файлы, скрипты или ассеты Maktala не используются. Этот репозиторий содержит только декомпиляцию Maktala в корне (для справки) — **наша игра живёт строго в папке `Vymesy/`**.

---

## Структура

```
Vymesy/
├── Assets/
│   └── Scripts/
│       ├── Achievements/      — система достижений и наград
│       ├── Audio/             — менеджер звуков и музыки
│       ├── Core/              — GameManager, RunManager
│       ├── Damage/            — IDamageable, DamageInfo, DamageSystem
│       ├── Enemies/           — Вымесы: AI, спавн, типы, дроп лута
│       ├── Gems/              — гемы (4 слота), уровни, ребролл
│       ├── Inventory/         — предметы (6 слотов), редкости, авто-продажа
│       ├── Items/             — мирные пикапы (магнит, подбор)
│       ├── MetaTree/          — дерево мета-улучшений
│       ├── Player/            — контроллер, статы, валюты, здоровье
│       ├── Pooling/           — ObjectPooler
│       ├── Projectiles/       — снаряды (прямые, самонаводящиеся, орбита)
│       ├── Save/              — JSON SaveLoad, PlayerData
│       ├── Skills/            — авто-скиллы и теги
│       ├── Towers/            — башни (AoE, Circle, Gold, Poison)
│       ├── UI/                — HUD, главное меню, экран конца забега
│       ├── Utils/             — Singleton, EventBus, MathUtils
│       └── VFX/               — CameraShake, CameraFollow
├── Packages/manifest.json     — список нужных Unity-пакетов
├── ProjectSettings/           — заглушка с целевой версией Unity
└── .gitignore                 — исключения Unity
```

---

## Открытие в Unity (когда установишь)

1. Установи **Unity 2022.3 LTS** или **Unity 6000.x LTS** через [Unity Hub](https://unity.com/download). При установке выбери модули Android/Windows/Mac по желанию.
2. В Unity Hub → **Open** → укажи папку `Vymesy/` (именно её, не корень репо).
3. Unity создаст `Library/`, импортирует пакеты из `Packages/manifest.json` и скомпилирует все скрипты.
4. Если консоль ругается на `linearVelocity` (Unity 6 API) — игнорируй, скрипты переключаются автоматически через `#if UNITY_6000_0_OR_NEWER`.

---

## Быстрый запуск демо

1. В окне Project создай папку `Assets/Scenes`.
2. `File → New Scene` → выбери `2D` / `Basic 2D`.
3. `File → Save As...` → сохрани сцену как `Assets/Scenes/Game.unity`.
4. В Hierarchy: правый клик → `Create Empty`.
5. Переименуй объект в `VymesyDemoBootstrap`.
6. Выдели объект → Inspector → `Add Component` → найди `VymesyDemoBootstrap` → выбери `Vymesy.Demo.VymesyDemoBootstrap`.
7. Убедись, что `Auto Start Run` включён.
8. Нажми Play.

`VymesyDemoBootstrap` сам создаёт игрока, камеру, врагов, скиллы, башню, предметы и IMGUI-HUD без префабов и Canvas.

Управление: `WASD` — движение, `R` — рестарт, `B` — алтарь, `F1` — статистика.

Для сборки: `File → Build Settings...` → `Add Open Scenes` → выбери `PC, Mac & Linux Standalone` → `Build And Run`.

---

## Ручная настройка сцены

1. Создай две сцены: `MainMenu` и `Game` (File → New Scene → 2D, сохрани в `Assets/Scenes/`).
2. **`Game` сцена** — собери:
   - GameObject `[GameManager]` → компонент `Vymesy.Core.GameManager`. Префаб `RunManager` пока оставь пустым.
   - GameObject `[ProjectilesManager]` → компонент `Vymesy.Projectiles.ProjectilesManager` + дочерний `ObjectPooler`. В пул загляни → добавь префабы снарядов с компонентом `Projectile`.
   - GameObject `Player` (sprite renderer + Rigidbody2D + Collider2D) → компоненты `PlayerController`, `PlayerHealth`, `PlayerManager`.
   - GameObject `[EnemiesManager]` → компонент `EnemiesManager` + дочерний `ObjectPooler`. В список `Entries` добавь `EnemyDefinition` ассеты + соответствующие префабы (с компонентами `EnemyController`, `EnemyHealth`, `EnemyAI`, `EnemyView`).
   - GameObject `[Skills/Inventory/Gems/Towers]` (можно один родитель) с соответствующими менеджерами.
   - GameObject `Main Camera` → компоненты `CameraFollow` (target = Player) и `CameraShake`.
   - Canvas с `HUDController`, `HealthBar`, `RunEndScreen`, `InventoryView`, опционально `MetaTreeView`.
3. **`MainMenu` сцена** — Canvas с `MainMenuController`. В поле `Game Scene Name` вписать имя сцены `Game`.
4. В **Build Settings** добавь обе сцены.

---

## Если Unity не скачивает пакеты

Если видишь ошибку вида `Cannot connect to download.packages.unity.com (ECONNRESET)`, проверь, что firewall/VPN/proxy разрешает доступ к:

```text
https://download.packages.unity.com
```

Проект не требует TextMeshPro, Test Framework или IDE-пакетов для запуска демо. `Packages/manifest.json` оставлен минимальным, чтобы сцена с `VymesyDemoBootstrap` открывалась даже при проблемах с необязательными пакетами.

Если Unity продолжает показывать старые ошибки после изменения пакетов, закрой Unity, удали папку `Library/` внутри `Vymesy/` и открой проект заново.

Если видишь `namespace 'Vymesy.Demo' already contains a definition for 'DemoBootstrap'`, в проекте остался старый/ручной скрипт `DemoBootstrap.cs`. Закрой Unity, удали или перенеси вне `Assets` все локальные `DemoBootstrap.cs`, затем используй компонент `VymesyDemoBootstrap`.

---

## Создание ассетов

В меню `Assets → Create → Vymesy/...`:
- **Enemies/Enemy Definition** — описание типа вымеса (HP, скорость, дроп).
- **Skills/Projectile Skill / AoE Skill / Orbit Skill** — авто-скиллы.
- **Towers/Tower Definition** — описание башни.
- **Items/Item** — предмет с модификатором статов.
- **Gems/Gem** — гем со статом и значением за уровень.
- **Meta/Tree Node** — узел дерева мета-улучшений.
- **Achievements/Achievement** — достижение.

---

## Архитектура — кратко

- **GameManager** (Singleton) — единый вход, грузит `PlayerData` из JSON.
- **RunManager** — жизненный цикл забега; владеет всеми `*Manager` подсистемами.
- **EventBus** — статическая шина типизированных событий (`PlayerDamagedEvent`, `EnemyKilledEvent`, `RunStartedEvent`, …). Подписки в `OnEnable`, отписки в `OnDisable`.
- **ObjectPooler** — единственный пул, переиспользуется снарядами, врагами, пикапами.
- **PlayerStats / PlayerStatsModifier** — базовые статы + аддитивные модификаторы. Предметы, гемы и узлы дерева кладут модификаторы; `PlayerManager.RebuildStats()` пересобирает финальные значения.
- **DamageSystem** — единственная точка применения крита и множителей урона игрока.
- **SaveLoadManager** — JSON в `Application.persistentDataPath/vymesy_save.json`. Словари сериализуются через `SaveWrapper` (две параллельные коллекции).

---

## Свободные ассеты

- 🎨 [Kenney.nl](https://kenney.nl/assets) — спрайты, иконки, тайлсеты.
- 🎮 [itch.io free assets](https://itch.io/game-assets/free) — тёмное фэнтези пакеты.
- 🔊 [freesound.org](https://freesound.org) — звуки.
- 🎵 [opengameart.org](https://opengameart.org) — музыка.

> Перед использованием **обязательно** проверяй лицензию (CC0 / CC-BY) и добавляй автора в `CREDITS.md`.

---

## Дальнейшие шаги

- [ ] Собрать `Game` сцену и проверить базовый цикл (старт → смерть → конец).
- [ ] Сделать первые `EnemyDefinition` для трёх вымесов: Common / Stalker / Brute.
- [ ] Сделать стартовый `ProjectileSkill` (например — "Светлая стрела") и привязать к новому игроку.
- [ ] Импортировать ассеты с Kenney.nl / itch.io под тёмное фэнтези.
- [ ] Добавить шейдер/частицы для эффекта смерти вымеса.
- [ ] Балансировка волн и усложнения.
