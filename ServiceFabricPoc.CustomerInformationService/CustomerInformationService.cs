﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Domain = ServiceFabricPoc.CustomerInformation.Domain;

namespace ServiceFabricPoc.CustomerInformationService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class CustomerInformationService : StatefulService
    {
        public CustomerInformationService(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
        
        public async Task<IEnumerable<Domain.CustomerInformation>> GetCustomerInformationAsync(CancellationToken ct)
        {

            IList<Domain.CustomerInformation> results = new List<Domain.CustomerInformation>();
            /*
            IReliableDictionary<InventoryItemId, InventoryItem> inventoryItems =
                await this.StateManager.GetOrAddAsync<IReliableDictionary<InventoryItemId, InventoryItem>>(InventoryItemDictionaryName);

            ServiceEventSource.Current.Message("Called GetCustomerInventory to return InventoryItemView");

            await this.PrintInventoryItemsAsync(inventoryItems, ct);


            using (ITransaction tx = this.StateManager.CreateTransaction())
            {
                ServiceEventSource.Current.Message("Generating item views for {0} items", await inventoryItems.GetCountAsync(tx));

                IAsyncEnumerator<KeyValuePair<InventoryItemId, InventoryItem>> enumerator =
                    (await inventoryItems.CreateEnumerableAsync(tx)).GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(ct))
                {
                    if (enumerator.Current.Value.AvailableStock > 0)
                    {
                        results.Add(enumerator.Current.Value);
                    }
                }
            }

            */
            return results;
        }
    }
}
