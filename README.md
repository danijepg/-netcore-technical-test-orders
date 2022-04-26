**Good luck!**

Adjunto una explicación de la arquitectura que he visto en el código y diversos cambios que se podrían hacer. Pero antes indicar que en las 1-2 horas que indica el pdf de las instrucciones no he tenido tiempo a realizar ninguna implementación concreta, teniendo en cuenta además (aunque esto quizá sea culpa mia) que he tenido que configurar un entorno). Quizás me este perdiendo algo muy obvio.

**Comandos y Eventos**

Lo primero que cambiaria sería añadir las implementaciones base de comando y evento, que se necesitarían luego para casi cualquier cosa generica que quieras hacer, como validaciones, o modificaciones del agregado via eventos.

En el agregado hay dos cosas que podrían tener sentido, la primera es un campo versión, en caso de que se quiera realizar una gestión de bloqueos optimista, y quizá un campo decidir el ratio de snapshoteo del agregado (lo explico más adelante)

**Modelo de escritura**

No me ha quedado claro como se quiera guardar el agregado. Por el ejemplo de insert parece que se guarda un jsonb directamente con los campos. Me queda la duda de que se pretende hacer con el modelo de escritura o si tgengo que plantearlo. Lo que yo tendría en mente probablente sería:

    - Se almacenan los agregados en la tabla de agregados tal y como aparecen en el insert, estos agregados, que en el fondo es el agregado con toda su lista de eventos ordenada (me falta el campo secuencia también). 

    - Hace falta además una cosa que no he visto, un modelo de escritura. Ya sea en memoria (lo cual estaria bien dependiento de los datos) o en bbdd. Este modelo contiene basicamente la colección con los agregados y es lo que se usuaríaa para las consultas de escritura. Por ejemplo comprobar repetidos, validaciones y resto de operaciones que pueden llegar via comandos y que se tienen que relizar contra escritura. 

    - Este modelo de escritura se puede recrear al arrancar el sericio desde los agregados que estan almacenados aplicando los eventos. Es muy posible que dependiendo del agregado no se quiera recrear usando todos los eventos (o de hecho ninguno), por ejemplo aquellos que tengan muchas modificaciones por cuestiones de rendimiento. Para lo que podría tener sentido un valor de configuración en cada agregado. Evidentemente si ademas de los eventos de cada agregado guardas en una tabla el ultimo snapshot, ya no necesitas recrearlo al arrancar. Pero si te vas a una base de datos en memoria, que puede tener sentido si tienes muchos accesos a un dato que cambia poco (y te ahorras accesos a base de datos que son mas caros) puede tener sentido.

El acceso al repositorio o contexto de escritura podría realizarse desde el command handler.

**Agregado**

Debería tener un handler para atender a los eventos y realizar las modificaciones necesarias en sus campos.


**Buses**

En la inserción del evento en el bus, puede ser interesante tener más de un bus. Por ejemplo un  bus de validación que permita realizar los validaciones de los datos sin tener que hacerlo en el handler, o un bus de operaciones pre o post. Ademas estos buses se puede considerad su sincronicidad. Por ejemplo un bus de validación debería ser claramente síncrono, pero un bus para logear las entidades o recoger estadísticas podría ser asíncrono.

**Publicación de evento**

Una vez almacenados los agregados y existiendo un modelo de escritura, la publicación del evento en rabbitMq esta clara, pero tampoco me ha quedado claro si lo que se pretende luego es llevar eso a un modelo de lectura también en postgresql (y supuestamente desnormalizado o similar) de forma que las queries de pantallas etc, se puedan hacer contra este modelo. 

**Modelo de lectura?**

En ese caso puede aparecer otro problema, que es el de la desincronización entre lectura y escritura. En el caso de que el proceso que lleve los datos a la base de datos de escritura sea más lento que el de escritura. Esto, es fácil que suceda por varias cosas:

    - El modelo de escritura suele contener datos vivos, mientas que el de lectura es típico dejar datos vivos, pero también historicos, lo que termina haciendo que las queries de inserción sean más lentas (por ejemplo por los propios indices que se añadan para las queries)
    - Para los datos de lectura se puede caer en la tentación de ejecutar los inserts en commits para solucionar el problema anterios. En caso de bases de datos relacionales este valos hay que escogerlo con cuidado ya puede terminar en commits lentos que produzcan una cola.
    - La posibilidad de realizar inserciones en paralelo puede ser compleja ya que en sistemas grandes es díficil entender si un evento puede llegar a machacar los datos de otro.

En el caso de que se pretendise llevar los datos de mensajeria a una base de datos relacional como menciono arriba, hay que tener en cuenta también que a pesar de que esa base de datos se mantiene con eventos, puede resultar interesante realizar una proyección para solucionar posibles desajustes producidos por bugs, o corrupción de la base de datos.

En cuanto a las preguntas especificas.

**Is it possible that even when order service responds to a request with a 201 status code response the api client sends the same request again -for example as part of a retry policy on the api client side??**

Si, es perfectamente posible. Básicamente habría dos solcuiones. Primero, un bloqueo del agregado, por ejemplo en el command handler. Esto haría que antes de comprobar si ya existe un elemento con ese id se quedara bloqueado esperando a que cualquier otro insert acabara.

La otra posibilidad sería un optimistic lock, que se gestionaría con una número de versión. En caso de conflicto existen a su vez varias posibles soluciones. Tanto a gestionar por el cliente (refrescar y reenviar) como por el servidor (recuperar los eventos y ver si realmente existe un conflicto y en su caso reintentar con un número de versión superior)

**Is it possible to publish messages in a different order than the one in which their respective orders were processed and persisted? If so, how can we avoid it? What trade-offs are considered?**

Depende de la gestión de las colas. Una posibilidad es siempre arrastrar un número de secuencia. Los eventos pueden ser siempre recuperados usando este orden a posteriori. El orden de los eventos es relevante ya que la información se puede sobreescribir.

**Is it possible to publish message duplicates?**

En el caso de que la publicación del evento se gestione correctamente despues de la comprobación de unicidad, no.

**Is it possible to consume messages out of order? If so, how can we avoid it?**

Como se explico usando un número de suencia para los eventos

**Is it possible to consume message duplicates? If so, how can we avoid it**

No

**Message semantics**

**Which are the semantic differences?**

El comando se podría considerar una petición de una acción (casualmente en el esqueleto el parámetro del comando se llama request) mientras que el evento representa algo que ya sucedio y no debería ser cambiado.

**Does service interaction change in a meaningful way?**


**Is there a need to introduce additional components or change system topology?**

Para las dos preguntas anteriores depedería de varios factores, la propia naturaleza del evento hace que sea lo indicado para el caso en que varios servicios esten interesandos en la información.  En el caso propuesto el servicio de shipping podría crear un envio. Pero el mismo evento podría ser de interes para otros servicios.