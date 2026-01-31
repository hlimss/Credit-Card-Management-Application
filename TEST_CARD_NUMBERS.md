# Numéros de carte de test pour la validation

Pour tester l'application, utilisez ces numéros de carte qui passent la validation Luhn :

## Numéros de carte valides pour les tests

### Visa
- **4111 1111 1111 1111** (Visa classique) ✅ VALID
- **4242 4242 4242 4242** (Visa) ✅ VALID
- **4000 0000 0000 0002** (Visa) ✅ VALID

### MasterCard
- **5555 5555 5555 4444** (MasterCard) ✅ VALID
- **5105 1051 0510 5100** (MasterCard) ❌ INVALID - Ne pas utiliser
- **5424 0000 0000 0015** (MasterCard) ✅ VALID

### American Express
- **3782 822463 10005** (American Express - 15 chiffres)
- **3714 496353 98431** (American Express)

### Discover
- **6011 1111 1111 1117** (Discover)
- **6011 0009 9013 9424** (Discover)

## Format de date d'expiration

Utilisez le format **MM/YY** avec une date dans le futur :
- ✅ **12/25** (décembre 2025)
- ✅ **06/26** (juin 2026)
- ❌ **01/20** (janvier 2020 - expiré)

## CVV

Utilisez 3 ou 4 chiffres :
- ✅ **123** (3 chiffres)
- ✅ **1234** (4 chiffres pour Amex)

## Exemple complet pour tester

```
Card Number: 4111 1111 1111 1111
Cardholder Name: John Doe
Expiration Date: 12/25
CVV: 123
```

## ⚠️ IMPORTANT

**Tous les numéros de carte doivent passer la validation Luhn pour être acceptés.**

Si vous obtenez l'erreur "Invalid card number", c'est que le numéro ne passe pas la validation Luhn. Utilisez uniquement les numéros marqués ✅ VALID dans cette liste.

## Notes importantes

1. **Ces numéros sont uniquement pour les tests** - Ne les utilisez jamais pour de vraies transactions
2. La validation Luhn vérifie que le numéro de carte est mathématiquement valide
3. La date d'expiration doit être dans le futur
4. Le format MM/YY est obligatoire (avec le slash)
