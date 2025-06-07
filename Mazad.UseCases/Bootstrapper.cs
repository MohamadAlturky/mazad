using Mazad.UseCases.Categories.Create;
using Mazad.UseCases.Categories.Delete;
using Mazad.UseCases.Categories.Read;
using Mazad.UseCases.Categories.Toggle;
using Mazad.UseCases.Categories.Update;
using Microsoft.Extensions.DependencyInjection;

namespace Mazad.UseCases;

public static class Bootstrapper
{
    public static IServiceCollection AddUseCasesServices(
        this IServiceCollection services
    )
    {
        // Categories services
            /// commands
        services.AddScoped(typeof(CreateCategoryCommandHandler));
        services.AddScoped(typeof(UpdateCategoryCommandHandler));
        services.AddScoped(typeof(DeleteCategoryCommandHandler));
        services.AddScoped(typeof(ToggleCategoryActivationCommandHandler));

            /// queries
        services.AddScoped(typeof(GetAllCategoriesQueryHandler));
        services.AddScoped(typeof(GetCategoriesDropdownQueryHandler));
        services.AddScoped(typeof(GetCategoriesListQueryHandler));
        services.AddScoped(typeof(GetCategoriesTreeByOneQueryHandler));
        return services;
    }
}
