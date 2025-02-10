using Basket.API.Data;
using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddMarten(opt =>
{
    opt.Connection(builder.Configuration.GetConnectionString("PostgresConnection")!);
    opt.AutoCreateSchemaObjects = Weasel.Core.AutoCreate.CreateOrUpdate;
    opt.Schema.For<ShoppingCart>().Identity(x => x.UserName);   //configure username is identity of table, can use [identity]
}).UseLightweightSessions();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();

//with using records (Records use constructor-based mapping) - classes with parameterless not need config
TypeAdapterConfig<StoreBasketResult, StoreBasketResponse>
    .NewConfig()
    .MapWith(src => new StoreBasketResponse(src.userName));

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

app.MapCarter();

app.UseExceptionHandler(options => { });


app.Run();
