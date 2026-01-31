# Instructions pour SQL Server

## Après l'installation de SQL Server 2025

### Option 1: SQL Server LocalDB (Recommandé pour développement)
Si vous avez installé SQL Server avec LocalDB, la configuration actuelle fonctionnera automatiquement :
```
Server=(localdb)\mssqllocaldb;Database=CreditCardDB;Trusted_Connection=True;MultipleActiveResultSets=true
```

### Option 2: SQL Server Express/Standard
Si vous utilisez SQL Server Express ou Standard, modifiez `appsettings.json :

**Pour SQL Server Express (instance par défaut):**
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=CreditCardDB;Trusted_Connection=True;MultipleActiveResultSets=true"
```

**Pour SQL Server Standard (instance nommée):**
```json
"DefaultConnection": "Server=localhost\\VOTRE_INSTANCE;Database=CreditCardDB;Trusted_Connection=True;MultipleActiveResultSets=true"
```

**Avec authentification SQL Server:**
```json
"DefaultConnection": "Server=localhost;Database=CreditCardDB;User Id=sa;Password=VOTRE_MOT_DE_PASSE;Trusted_Connection=False;MultipleActiveResultSets=true"
```

## Vérifier que SQL Server fonctionne

1. Ouvrez SQL Server Management Studio (SSMS)
2. Connectez-vous à votre instance SQL Server
3. Vérifiez que vous pouvez vous connecter

## Lancer l'application

Une fois SQL Server installé et configuré :

```bash
cd CreditCardManagement.API
dotnet restore
dotnet run
```

L'application créera automatiquement la base de données `CreditCardDB` au premier démarrage.
