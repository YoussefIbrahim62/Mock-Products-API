var builder = WebApplication.CreateBuilder(args);

// 1. إضافة الخدمات (Services)
builder.Services.AddControllers();

// ---> التأكد من وجود هذين السطرين لإضافة Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. تفعيل الـ Middleware لـ Swagger
if (app.Environment.IsDevelopment())
{
    // ---> التأكد من وجود هذين السطرين لتشغيل واجهة Swagger
    app.UseSwagger();
    app.UseSwaggerUI(); // يتيح الوصول لـ /swagger/index.html
}

app.UseAuthorization();
app.MapControllers();

app.Run();