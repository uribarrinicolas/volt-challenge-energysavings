# Instrucciones

Este desarrollo se realizó según las especificaciones del challenge de Volt. En el mismo se solicitaba que fuese dockerizado y se epecifique en un instructivo cómo utilizar el programa.

Esta aplicación utiliza archivos de entrada y escribe un archivo de salida. Es por eso que se solicitará que se utilice un volumen de docker para interactuar con el fs del contenedor.

**IMPORTANTE: Para lo que es el desarrollo del challenge, se mantendrá como premisa que los archivos de entrada serán listas ordenadas con la misma cantidad de objetos y que corresponderán a los mismos minutos. No entrará en el alcance de este desarrollo el manejo de errores relacionado a este tema.**

## Paso a paso

Ejecutar estando en el directorio del Dockerfile

```
docker build -t uribarri/voltchallenge-energysavings .
```

Para ejecutar el contenedor creando un volumen en el directorio actual (suponiendo que se esta ejecutando en unix) se puede utilizar el siguiente comando.

```bash
docker run --rm -v $(pwd):/data uribarri/voltchallenge-energysavings /data/miami_household_consumption_with_timestamps.json /data/miami_solar_output_with_timestamps.json /data/energy_report.json
```

Para el comando anterior se debe tener en cuenta que los archivos que esten en `pwd` (directorio actual), al pasarlos como args, se debe considerar el directorio interno del volumen. En este caso, el directorio interno es `/data` y, por lo tanto, los archivos que se encuentran en `pwd` en el host, se deben pasar con prefijo `/data/` como se ve en los args enviados en el comando.

El tercer arg indica el objeto resultado.

*Para ejecutar en `PowerShell` es suficiente reemplazar `$(pwd)` por `${PWD}`*
