# Activitat-Inventari

Actividad 2: Inventario
Enunciado del ejercicio
Sistema de inventario para un RPG en Unity con SQLite
En esta práctica se desarrollará un sistema de inventario persistente para un videojuego
de rol (RPG) utilizando Unity y una base de datos SQLite local. El objetivo principal es
aplicar los conocimientos de acceso a datos mediante el uso de bases de datos integradas
en aplicaciones.
El sistema permitirá gestionar los objetos que el jugador puede poseer durante la partida.
Estos objetos tendrán información propia que los identifique dentro del juego y describa su
función. Algunos objetos podrán ser acumulables, mientras que otros no. El inventario
deberá reflejar en todo momento el estado actual de los objetos del jugador.
La información deberá almacenarse en una base de datos local, de forma que:
● El inventario se conserve al cerrar y volver a abrir el juego.
● El inventario es dependiente del usuario que hace login.
● Los cambios realizados durante la partida queden guardados.
● La estructura de la base de datos permita representar correctamente la información
del sistema de inventario.
Diseño de la base de datos
Antes de la implementación, el alumnado deberá realizar el diseño de la base de datos,
identificando:
● La información que es necesario almacenar para el inventario.
● Cómo se relacionan los distintos datos entre sí.
● Qué información depende de otra y no puede existir de forma independiente.
