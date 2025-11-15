# Sistema de Control de Entrada y Salida con QR  
  
Sistema web para gestión de accesos mediante códigos QR, desarrollado con ASP.NET Core 8.0 y arquitectura en capas.  
  
## 🚀 Características  
  
- **Gestión de Usuarios**: Registro y administración de usuarios del sistema  
- **Códigos QR Dinámicos**: Generación de credenciales QR para control de acceso  
- **Tres Módulos de Usuario**:  
  - Módulo de Administradores  
  - Módulo de Guardias de Seguridad  
  - Módulo de Usuarios  
- **Control de Accesos**: Registro de entradas y salidas  
- **Gestión de Tutores**: Soporte para tutores legales y temporales  
  
## 🏗️ Arquitectura  
  
El proyecto implementa una arquitectura en capas:

Sistema_Control_Entrada_Salida_QR/ # Capa de presentación (Razor Pages)
├── CarnetDigital.Services/ # Lógica de negocio
├── CarnetDigital.Repository/ # Acceso a datos
└── CarnetDigital.Entities/ # Modelos de dominio

## 📋 Requisitos Previos  
  
- .NET SDK 8.0 o superior  
- SQL Server (o compatible)  
- Visual Studio 2022+ / VS Code 
  
## 🛠️ Tecnologías
Framework: .NET 8.0
Web: ASP.NET Core Razor Pages
Base de Datos: SQL Server
Arquitectura: Capas (Presentation, Business Logic, Data Access, Domain)

## 📁 Estructura del Proyecto
Sistema_Control_Entrada_Salida_QR: Aplicación web principal con Razor Pages
Modulo Administradores/: Páginas de administración
Modulo Guardas/: Páginas para personal de seguridad
Modulo Usuarios/: Páginas para usuarios finales
wwwroot/images/: Almacenamiento de fotos y códigos QR
CarnetDigital.Services: Servicios de negocio y validaciones
CarnetDigital.Repository: Repositorios y acceso a datos
CarnetDigital.Entities: Entidades del modelo de dominio

