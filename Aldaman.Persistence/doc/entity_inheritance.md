# Entity Inheritance Hierarchy

This document visualizes the inheritance hierarchy of base and concrete domain entities in `Aldaman.Persistence.Entities`.

## Overview Diagram

```mermaid
classDiagram
    direction TB

    class BaseEntity {
        <<abstract>>
        +Guid Id
    }

    class BaseEntityCreatable {
        <<abstract>>
        +DateTime CreatedAtUtc
        +Guid? CreatedByUserId
        +AppUser? CreatedByUser
    }

    class BaseEntityCreatableSoftDel {
        <<abstract>>
        +bool IsDeleted
        +DateTime? DeletedAtUtc
        +Guid? DeletedByUserId
        +AppUser? DeletedByUser
    }

    class BaseEntityAuditable {
        <<abstract>>
        +DateTime? UpdatedAtUtc
        +Guid? UpdatedByUserId
        +AppUser? UpdatedByUser
    }

    class BaseEntityAuditableSoftDel {
        <<abstract>>
        +bool IsDeleted
        +DateTime? DeletedAtUtc
        +Guid? DeletedByUserId
        +AppUser? DeletedByUser
    }

    %% Base inheritance
    BaseEntity <|-- BaseEntityCreatable
    BaseEntityCreatable <|-- BaseEntityCreatableSoftDel
    BaseEntityCreatable <|-- BaseEntityAuditable
    BaseEntityAuditable <|-- BaseEntityAuditableSoftDel

    %% Concrete entities
    BaseEntityCreatableSoftDel <|-- ContactMessageEntity : inherits
    
    BaseEntityAuditable <|-- ContentTranslationEntity : inherits
    BaseEntityAuditable <|-- ContentGroupTranslationEntity : inherits
    BaseEntityAuditable <|-- ContentGroupContentEntity : inherits

    BaseEntityAuditableSoftDel <|-- ContentEntity : inherits
    BaseEntityAuditableSoftDel <|-- ContentGroupEntity : inherits
    BaseEntityAuditableSoftDel <|-- MediaAssetEntity : inherits
    BaseEntityAuditableSoftDel <|-- StyleSettingEntity : inherits
```

---

## Detailed Tree View

```text
BaseEntity (Id)
└── BaseEntityCreatable (CreatedAtUtc, CreatedByUserId, CreatedByUser)
    │
    ├── BaseEntityCreatableSoftDel (IsDeleted, DeletedAtUtc, DeletedByUserId, DeletedByUser)
    │   └── ContactMessageEntity
    │
    └── BaseEntityAuditable (UpdatedAtUtc, UpdatedByUserId, UpdatedByUser)
        │
        ├── ContentTranslationEntity
        ├── ContentGroupTranslationEntity
        ├── ContentGroupContentEntity
        │
        └── BaseEntityAuditableSoftDel (IsDeleted, DeletedAtUtc, DeletedByUserId, DeletedByUser)
            ├── ContentEntity
            ├── ContentGroupEntity
            ├── MediaAssetEntity
            └── StyleSettingEntity
```

---

## Entity Classification Summary

| Base Class | Properties Added | Concrete Entities |
|---|---|---|
| `BaseEntity` | `Id` | *(None directly)* |
| `BaseEntityCreatable` | `CreatedAtUtc`, `CreatedByUserId`, `CreatedByUser` | *(None directly)* |
| `BaseEntityCreatableSoftDel` | `IsDeleted`, `DeletedAtUtc`, `DeletedByUserId`, `DeletedByUser` | `ContactMessageEntity` |
| `BaseEntityAuditable` | `UpdatedAtUtc`, `UpdatedByUserId`, `UpdatedByUser` | `ContentTranslationEntity`, `ContentGroupTranslationEntity`, `ContentGroupContentEntity` |
| `BaseEntityAuditableSoftDel` | `IsDeleted`, `DeletedAtUtc`, `DeletedByUserId`, `DeletedByUser` | `ContentEntity`, `ContentGroupEntity`, `MediaAssetEntity`, `StyleSettingEntity` |

> [!NOTE]
> `AppUser` and `AppRole` are ASP.NET Core Identity entities inheriting from `IdentityUser<Guid>` and `IdentityRole<Guid>` respectively.
