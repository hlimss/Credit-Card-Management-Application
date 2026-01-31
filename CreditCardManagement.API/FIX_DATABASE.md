# Correction de la base de données

## Problème identifié

Le champ `CardNumber` dans la base de données avait une limite de **19 caractères**, mais après encryption Base64, un numéro de carte de 16 chiffres devient **24 caractères**, ce qui causait une erreur lors de l'insertion.

## Solution appliquée

Les limites ont été augmentées dans le modèle :
- `CardNumber` : 19 → **500 caractères**
- `CVV` : 4 → **100 caractères**

## Action requise

La base de données doit être recréée avec les nouvelles colonnes. Deux options :

### Option 1 : Supprimer et recréer la base (Recommandé)

1. Ouvrez SQL Server Management Studio (SSMS)
2. Connectez-vous à `localhost`
3. Supprimez la base de données `CreditCardDB` :
   ```sql
   DROP DATABASE CreditCardDB;
   ```
4. Redémarrez l'application backend - elle recréera automatiquement la base avec les bonnes colonnes

### Option 2 : Modifier les colonnes existantes

Si vous voulez garder les données existantes, exécutez ces commandes SQL :

```sql
USE CreditCardDB;
GO

ALTER TABLE CreditCards
ALTER COLUMN CardNumber NVARCHAR(500) NOT NULL;
GO

ALTER TABLE CreditCards
ALTER COLUMN CVV NVARCHAR(100) NOT NULL;
GO
```

## Vérification

Après avoir appliqué la correction, essayez de créer une carte avec :
- Numéro : `4111 1111 1111 1111`
- Nom : `Test User`
- Date : `12/26`
- CVV : `123`

Cela devrait maintenant fonctionner !
