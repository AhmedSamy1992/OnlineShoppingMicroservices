using Basket.API.Data;
using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using Discount.Grpc;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

//application services
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

//Data services
builder.Services.AddMarten(opt =>
{
    opt.Connection(builder.Configuration.GetConnectionString("PostgresConnection")!);
    opt.AutoCreateSchemaObjects = Weasel.Core.AutoCreate.CreateOrUpdate;
    opt.Schema.For<ShoppingCart>().Identity(x => x.UserName);   //configure username is identity of table, can use [identity]
}).UseLightweightSessions();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>(); // === new CachedBasketRepository(new BasketRepository())

//using scrutor library rather than register decorator class manually

//builder.Services.AddScoped<IBasketRepository>(provider =>
//{
//    var basketRepository  = provider.GetRequiredService<BasketRepository>();
//    return new CachedBasketRepository(basketRepository, provider.GetRequiredService<IDistributedCache>());
//});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

//with using records (Records use constructor-based mapping) - classes with parameterless not need config
TypeAdapterConfig<StoreBasketResult, StoreBasketResponse>
    .NewConfig()
    .MapWith(src => new StoreBasketResponse(src.userName));


//Grpc services
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
});

//cross-cutting concern services
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

app.MapCarter();

app.UseExceptionHandler(options => { });


app.Run();
