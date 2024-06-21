# Paso a paso y reflxión mental sobre la resolución del challenge

Si bien el challenge no especifica que sea necesario adjuntar el proceso del *cómo* se toman las decisiones a lo largo del desarrollo, yo considero que es información que puede resultar interesante a alguno de los actores involucrados, y al mismo tiempo me permite evaluar mi propio trabajo en retrospectiva.

Es por eso que creo y mantengo este documento en donde estaré listando las reflexiones y decisiones que considere pertinentes sobre el challenge.

## Simplicidad

Al iniciar el análisis de un proyecto o solución uno de los primeros puntos que encaro es el bajo qué contexto se desarrollará y mantendrá el mismo (los requisitos no funcionales que hacen a la organización pueden condicionar la solución), qué nivel de "madurez" tendrá (si debe cumplir con estándares específicos) y cuál sera la técnica para escalar el desarrollo (si se trata de servicios descartables ante un cambio o es necesario que se mantenga en el tiempo con los incrementos posteriores).

Por una cuestión de velocidad de desarrollo y para no alejarnos mucho de lo solicitado en el challege, creo que ulizar el template base de `console` de `dotnet new` es lo ideal. No voy a estar haciendo tampoco un gran árbol de directorios para resolver el problema ya que la intención es que sea simple y atómico para ser candidato a migrar a una `lambda` o equivalente.

## Versionado

Si bien no esta pedido en el challenge que el resultado sea publicado en un repositorio, sino en una carpeta compartida, en lo personal prefiero mantener todo en repositorios git, así sea de forma local. Esto también me permite hacer retrospectiva de mi código revisando el historial de commits.

## Proceso

El proceso de desarrollo lo planifico de la siguiente forma:

- [Análisis de la solución](#análisis-de-la-solución), entender el challenge, sus entradas y salidas como proceso *"validar con el cliente"*, incluso diagramar algo para entender el universo al que nos enfrentamos. Esto permite que encontremos errores conceptuales en una etapa temprana y también encontremos la solución *ideal* en menos tiempo y *-potencialmente-* ahorremos tiempo de desarrollo solucionando estos errores u optimizaciones.
- [Desarrollo en *raw*](#desarrollo-en-raw), código rápido y crudo, buscando la solución en poco tiempo a cualquier costo (computacional). Esta solución se basa en el análisis, por lo tanto no debería variar mucho de la solución ideal. Sin embargo, esta solución no tendrá separación en capas de la misma forma que el resultado final, no necesariamente tendrá las interfaces de forma prolija, etc. En casos en donde se apliquen tests unitarios (no exactamente este challenge), se pueden ir desarrollando los mismos para garantizar mantener la integridad del código en los próximos pasos (aproximación muy parecido a lo que se ve en `TDD`).
- [Optimización](#optimización), ya que nuestro código podría no tener la complejidad deseada, una vez validado el funcionamiento podemos encarar modificaciones que reduzcan el *Big-O* y mejoren el uso de nuestros recursos (todavía en una solución *"fea"*).
- [Refactor](#refactor), ya que nuestro código esta lejos de ser el más lindo de la feria en esta instancia. Teniendo en cuenta que se realizó en el menor tiempo posible podríamos encontrarnos situaciones como que estemos trabajando en un único archivo, sin separar responsabilidades, con código duplicado. En este paso apuntamos a tener una aplicación en consola (como el challenge indica) en un estado presentable y publicable como tal, pero manteniendo la [simplicidad](#simplicidad).
- [Dockerización](#dockerización) de la aplicación de consola, ya que el challenge indica que el resultado debe ser dockerizado y será ejecutado en una máquina utilizando linux, en este paso se va a modificar la interfaz de entrada para que los parámetros tengan sentido dentro de la ejecución virtual. Se desarrollará tanto el `Dockerfile` como un documento donde indique como construir la imagen y como ejecutarla, contemplando el uso de volúmenes para permitir el uso de archivos (tanto los de entrada como el informe de salida).

### Análisis de la solución

El análisis de solución arranca con la construcción de este documento.

A continuación, la lectura en detalle del challenge, el análisis de los archivos de entrada (principalmente el esquema de los archivos json), la matriz de configuración asociada al costo de electricidad, el proceso de transformación de la información y el formato del informe de salida.

Con respecto al costo de electricidad, se define que se realizará una función que dado un timestamp devuelva el costo. Esta función será estática en el código y no se contemplará para el desarrollo del challenge la posibilidad de que sea ingresada una matriz o lista de objetos en un archivo de configuración o parámetros. Una funcionalidad como esa podría añadirse para un potencial incremento de la aplicación.

Con respecto a los archivos de entrada, me encuentro que ambos archivos se trata de una lista de minutos a lo largo de un año con un timestamp identificando al minuto más información específica. En el archivo `miami_household_consumption_with_timestamps.json` cuya propiedad `electricity_consumption` se trata de la electricidad consumida en **kw** en ese minuto y en el archivo `miami_solar_output_with_timestamps.json` la propiedad `solar_output` se trata del ingreso de electricidad que ingresa por los paneles solares en **kw** en ese minuto.

Aquí aparece una duda sobre si el hecho de tener o no baterías (tema que no se trata en la premisa) afecta el resultado del informe, ya que (según lo visto hasta ahora) podríamos tener un ingreso de electricidad por paneles solares superior a lo que necesitabamos consumir en ese minuto. ¿Qué sucede en ese caso? ¿Debe ignorarse el ingreso negativo o lo consideramos que se almacenó en algun lado para el uso posterior? ¿Se entrego al proveedor de energía y se toma como costo negativo con la misma taza que el proveedor ofrece? Entiendo que esta incertidumbre podría resolverse sola un poco mas adelante.

Con respecto al informe de salida, se tratará de un `json` (nombre del mismo ingresado por parametros) siguiendo la misma lógica de los archivos de entrada. El archivo consistirá de una lista minuto a minuto con las propiedades `timestamp`, `external_network_electricity_consumption` (el nombre de la propiedad lo considero claro para el challenge pero en un desarrollo para publicar buscaría un nombre un poco mas corto para reducir el overhead del `json` del informe) que tendrá la energía consumida del proveedor externo (resultado de la resta entre `electricity_consumption` y `solar_output`) y `savings` que tendrá el ahorro en $ (resultado de multiplicar `solar_output` por la tarifa correspondiente obtenida por la función de costo mencionada anteriormente, ponderando que se trata de kwh y nosotros tenemos un delta de 1 minuto).

#### IMPORTANTE: Premisa inicial

**Para lo que es el desarrollo del challenge, se mantendrá como premisa que los archivos de entrada serán listas ordenadas con la misma cantidad de objetos y que corresponderán a los mismos minutos. No entrará en el alcance de este desarrollo el manejo de errores relacionado a este tema.**

### Desarrollo en *raw*

Para iniciar el desarrollo parto de la base `dotnet new console -n EnergySavings` pero olvidé usar `--use-program-main` así que modifico manualmente el archivo `Program.cs`.

Desarrollo rapidamente los modelos para mapear los archivos de entrada y salida. Realizo la lectura de los archivos en disco sin desarrollar el procesamiento de args, sino que utilizo paths hardcodeados.

Busco imprimir en consola los objetos resultado de la lectura de los archivos antes de continuar para validar estar leyendo los archivos de forma correcta.

Una vez validado que se estan leyendo los archivos y mapeando correctamente con los modelos avanzo con el desarrollo de la función `GetKwhCost`. El nombre de la función, si bien podría modificarse en el refactor posterior, intenta ser claro en que lo que se obtiene es el valor del *kwh* (importante para no confundir la unidad) sin llegar a un nombre de funcion un poco mas claro pero que podría considerarse excesivo como `GetElectricityCostInKwh`.

Creo una función `GenerateEnergyReport` que se basa en la [premisa mencionada en el punto anterior](#importante-premisa-inicial) para recorrer una unico `for` manteniendo un `O(N)`.

Agrego la funcionalidad necesaria para que el reporte quede en disco en un archivo `json` con el formato esperado para el challenge.

### Optimización

Ya teniendo el resultado funcional listo pasamos a lo que será el análisis de complejidad de la solución en su estado actual.

Tenemos O(1) hasta las lecturas de los archivos, las cuales son O(N) considerando que el tamaño del archivo crece por cada entrada.

Pasamos a la función que genera el reporte, la funcion `GetKwhCost` quedo O(1) y teniendo en cuenta que la función `GenerateEnergyReport` recorre un solo `for` gracias a basarnos en la [premisa inicial](#importante-premisa-inicial), nos queda en O(N).

Por ultimo nos queda la función de escritura del reporte a un archivo, que la consideramos O(N).

Quedaría entonces O(1)+O(N)+O(N)+O(N*1)+O(N)=**O(N)**

Teniendo en cuenta que la complejidad del desarrollo quedo en O(N), no se buscará optimizar más allá el código.

### Refactor

Nuestro código es lo suficientemente óptimo para considerarse eficiente en un ambiente profesional (mi opinión), sin embargo, el código esta lejos de ser un código digno de presentar como ejemplar.

Extraemos las clases modelos a su propios archivos y bajo otro namespace.

Extraemos la funcionalidad del calculo del costo a su propio servicio najo un nuevo namespace. Tambien aplico algunos refactors sobre la coincidencia de patrones y miembros con cuerpo de expresión.

Extraemos la funcionalidad de generación de reporte a su propio servicio y extraemos la funcionalidad interna del for a su propia funcion `GenerateEnergyReportPerMinute`. Esta función tiene la lógica necesaria para eventualmente llevar esto a un procesamiento atómico, minuto por minuto a medida que ingresan los datos.

Convierto la función de escritura del archivo de reporte en generica y extraigo la funcinalidad de archivos a un helper.

Agrego la lógica para que tanto el nombre de los archivos de entrada como el nombre del archivo de salida se pase obligatoriamente args (y agrego al launchSettings los args de depuración).

Elimino logs de depuración.

Decido reducir overhead del reporte eliminando la opción de identado del proceso de serialización del json y la otra opción seteada es redundante ya que en el modelo se especifica la anotación `JsonPropertyName`. Elimino el `JsonSerializerOptions` en su totalidad del proceso de escritura el archivo.

### Dockerización

Se agrega un `Dockerfile` standard de .NET 8 y se agrega un documento instructivo para la utilización del proyecto contemplando el tema volúmenes de docker.
