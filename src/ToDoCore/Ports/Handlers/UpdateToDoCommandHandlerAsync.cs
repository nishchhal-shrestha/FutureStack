using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.logging.Attributes;
using paramore.brighter.commandprocessor.policy.Attributes;
using ToDoCore.Adaptors.Db;
using ToDoCore.Adaptors.Repositories;
using ToDoCore.Ports.Commands;

namespace ToDoCore.Ports.Handlers
{
    public class UpdateToDoCommandHandlerAsync : RequestHandlerAsync<UpdateToDoCommand>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public UpdateToDoCommandHandlerAsync(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }

        [RequestLoggingAsync(step: 1, timing: HandlerTiming.Before)]
        [UsePolicyAsync(policy: CommandProcessor.CIRCUITBREAKER, step:2)]
        [UsePolicyAsync(policy: CommandProcessor.RETRYPOLICY, step: 3)]
        public override async Task<UpdateToDoCommand> HandleAsync(UpdateToDoCommand command, CancellationToken cancellationToken = new CancellationToken())
        {

            using (var uow = new ToDoContext(_options))
            {
                var repository = new ToDoItemRepositoryAsync(uow);
                var toDoItem = await repository.GetAsync(command.ToDoId, cancellationToken);

                if (command.Title != null)
                    toDoItem.Title = command.Title;

                if (command.Complete.HasValue)
                    toDoItem.Completed = command.Complete.Value;

                if (command.Order.HasValue)
                    toDoItem.Order = command.Order.Value;

                await repository.UpdateAsync(toDoItem, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}