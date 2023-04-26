using Microsoft.Azure.Cosmos;
using MvcAzureCosmosDb.Models;

namespace MvcAzureCosmosDb.Services
{
    public class ServicesCosmosDb
    {
        //SE TRABAJA CON ITEMS CONTAINERS
        //DENTRO DE CONTAINER PODEMOS RECUPERAR EL CONTAINER
        //LLAMADA COSMOSCLIENT
        private CosmosClient client;
        public Container containerCosmos;

        public ServicesCosmosDb(CosmosClient client, Container containerCosmos)
        {
            this.client = client;
            this.containerCosmos = containerCosmos;
        }

        //TENDREMOS UN METODO PARA CREAR LA BBDD
        // Y DENTRO UN CONTENEDOR(LA TABLA)
        public async Task CreateDataBaseAsync()
        {
            //HEMOS DICHO QUE LA PK SERA ID
            //PERO PODEMOS INDICAR DE FORMA EXPLICITA QUE DICHA PK SERA OTRA
            ContainerProperties properties =
                new ContainerProperties("containercoches", "/id");
            //ESTO ES UN ITEMS CONTAINER
            await this.client.CreateDatabaseIfNotExistsAsync("vehiculoscosmos");
            //CREAMOS UN NUEVO CONTENEDOR DENTRO DE ITEMS CONTAINER
            await this.client.GetDatabase("vehiculoscosmos").CreateContainerIfNotExistsAsync(properties);

        }

        public async Task InsertVehiculosAsync(Vehiculo car)
        {
            //EN EL MOMENTO DE CREAR UN NUEVO ITEM
            //DEBEMOS INDICAR EL ITEM Y TAMBIEN SU PARTITION KEY EXPLICITAMENTE
            await this.containerCosmos.CreateItemAsync<Vehiculo>
                (car, new PartitionKey(car.Id));

        }

        //METODO PARA RECUPERAR TODOS LOS COCHES
        public async Task<List<Vehiculo>> GetVehiculosAsync()
        {
            //LOS DATOS SE RECUPERAN ATRAVES DE ITERATOR
            //NECESITAMOS RECORRER LOS ITEMS MIENTRAS EXISTAN
            var query =
                this.containerCosmos.GetItemQueryIterator<Vehiculo>();
            List<Vehiculo> coches = new List<Vehiculo>();
            while (query.HasMoreResults)
            {
                var resultados = await query.ReadNextAsync();
                //AÑADIMOS LOS COCHES QUE IRA RECUPERANDO A LA COLECCION
                coches.AddRange(resultados);
            }
            return coches;
        }

        //MODIFICAR
        public async Task UpdateVehiculoAsync(Vehiculo car)
        {
            await this.containerCosmos.UpsertItemAsync<Vehiculo>
                (car, new PartitionKey(car.Id));
        }

        public async Task DeleteVehiculoAsync(string id)
        {
            await this.containerCosmos.DeleteItemAsync<Vehiculo>
                (id, new PartitionKey(id));
        }

        //METODO BUSCAR VEHICULO
        public async Task<Vehiculo> FindVehiculoAsync(string id)
        {
            ItemResponse<Vehiculo> response  =
                await this.containerCosmos.ReadItemAsync<Vehiculo>
                (id, new PartitionKey(id));
            return response.Resource;
        }
    }
}
