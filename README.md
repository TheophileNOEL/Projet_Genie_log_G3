# 🚀 Projet_Genie_log_G3

[![.NET](https://img.shields.io/badge/.NET-6.0-blue)](https://dotnet.microsoft.com/)  
![Licence MIT](https://img.shields.io/badge/Licence-MIT-green)

**Groupe 3 – Génie Logiciel**  
Chef de projet : Théophile NOËL  
Équipe : Ylies CHAOUCHE, Lounis MOUALEK, Corentin ROMANO

---

## 📝 Description

**EasySave** est un outil de gestion de sauvegardes développé en C#/.NET.  
Il propose :
- Création, modification et suppression de « Travaux » de sauvegarde,
- Exécution sélective ou en file d’attente,
- Suivi d’état et génération de logs (JSON & XML),
- Cryptage des fichiers sensibles,
- Multilingue : Français & Anglais,
- Interface graphique riche sous WPF (EasySave 2.0) et console (v1.x).

---

## 🎯 Fonctionnalités clés

| Fonction                       | Console v1 | Console v1.1 | WPF v2.0 |
|:-------------------------------|:---------:|:------------:|:--------:|
| Multi-langues (FR / EN)        | ✓         | ✓            | ✓        |
| Nombre de travaux limité (5)   | ✓         | ✓            | ✕ (illimité) |
| Logs JSON                       | ✓         | ✓            | ✓        |
| Logs XML                        | ✕         | ✓            | ✓        |
| Modes de sauvegarde (Full, Diff) | ✓         | ✓            | ✓        |
| Cryptage des extensions         | ✕         | ✕            | ✓        |
| Détection automatique de l’exécutable métier (no-run) | ✕ | ✕  | ✓ |
| Interface graphique WPF         | ✕         | ✕            | ✓        |
| Ajout / Suppression de travaux  | ✓         | ✓            | ✓        |
| Exécution par lot ou manuelle   | ✓         | ✓            | ✓        |
| Barre de progression / statut   | ✕         | ✕            | ✓        |

---

## 🏗️ Architecture

Le projet est divisé en deux assemblies :

1. **EasySave.Core**  
   - Contient la logique métier : modèles (`Scenario`, `LogEntry`…), services (IO, cryptage, planification).
   - Génère les logs (JSON/XML), gère l’exécution et le suivi d’état.

2. **EasySave_G3_WPF**  
   - Front-end WPF en pattern MVVM : `MainWindow.xaml`, `ViewModel`.
   - Affiche les scénarios, permet la création/édition via formulaires, et le lancement des sauvegardes.
   - Utilise `FontFamily="Segoe MDL2 Assets"` pour les icônes système.

---

## ⚙️ Prérequis

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download) (ou plus récent)  
- Visual Studio 2022 (Community/Professional/Enterprise) avec workload **« Desktop Development with C#/.NET »**  

---

## 📥 Installation

```bash
# Cloner le dépôt
git clone https://github.com/votre-compte/Projet_Genie_log_G3.git
cd Projet_Genie_log_G3

# Ouvrir la solution dans Visual Studio
start Projet_Genie_log_G3.sln

# Ou en CLI (.NET CLI)
cd EasySave.Core
dotnet build
cd ../EasySave_G3_WPF
dotnet run
