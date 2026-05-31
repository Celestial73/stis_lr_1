# ER-диаграмма: медицинская клиника (ЛР-1)

## Диаграмма сущностей и связей

```mermaid
erDiagram
    SUSHCHNOST ||--o{ UCHASTNIK : ISA
    SUSHCHNOST ||--o{ USLUGA : ISA
    SUSHCHNOST ||--o{ PRIEM : ISA
    UCHASTNIK ||--o{ PATIENT : ISA
    UCHASTNIK ||--o{ DOCTOR : ISA

    PATIENT ||--o{ ZAPISYVAET : performs
    PRIEM ||--o{ ZAPISYVAET : target
    DOCTOR ||--o{ OKAZYVAET : performs
    PRIEM ||--o{ OKAZYVAET : target
    PRIEM ||--o{ SODERZHIT : includes
    USLUGA ||--o{ SODERZHIT : included
    PATIENT ||--o{ OPLACHIVAET : performs
    PRIEM ||--o{ OPLACHIVAET : target

    PATIENT {
        string imya
        string polis
    }
    DOCTOR {
        string imya
        string specializaciya
    }
    USLUGA {
        string nazvanie
        string tip
        int cena
    }
    PRIEM {
        string status
        date data
        string kabinet
    }
```

## ISA-иерархия концептов

```
Сущность
├── Участник
│   ├── Пациент
│   └── Врач
├── Услуга
└── Приём
```

## ISA-иерархия связей (опционально, п.7)

```
ДействиеПациента
├── ЗАПИСЫВАЕТ
└── ОПЛАЧИВАЕТ

ДействиеВрача
└── ОКАЗЫВАЕТ

СОДЕРЖИТ (корневая связь)
```

## Легенда

| Обозначение | Смысл |
|-------------|--------|
| ISA | отношение «является подтипом» (наследование концептов) |
| performs / target | направление связи между экземплярами |
| includes / included | приём содержит услугу |
