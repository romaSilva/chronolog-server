using Chronolog.Api.Application.Handlers;
using Chronolog.Api.Contracts;
using Chronolog.Api.Domain.Entities;
using Chronolog.Api.Infrastructure.Persistence;
using Chronolog.Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ChronologDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<NoteHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

static NoteResponse ToResponse(Note note) => new(
    note.Id,
    note.UserId,
    note.Title,
    note.Content,
    note.Year,
    note.Month,
    note.Day,
    note.DatePrecision,
    note.SortableDate,
    note.NoteTags.Select(nt => nt.Tag.NormalizedName)
);

app.MapPost("/notes", async (CreateNoteRequest request, NoteHandler handler, CancellationToken ct) =>
{
    var response = await handler.HandleAsync(request, ct);
    return Results.Created($"/notes/{response.Id}", response);
})
.WithName("CreateNote");

app.MapPut("/notes/{id:guid}", async (Guid id, UpdateNoteRequest request, NoteHandler handler, CancellationToken ct) =>
{
    var response = await handler.HandleAsync(id, request, ct);
    return response is null ? Results.NotFound() : Results.Ok(response);
})
.WithName("UpdateNote");

app.MapGet("/notes", async (INoteRepository noteRepository, CancellationToken ct) =>
{
    var notes = await noteRepository.GetAllAsync(ct);
    return Results.Ok(notes.Select(ToResponse));
})
.WithName("GetNotes");

app.MapGet("/notes/{id:guid}", async (Guid id, INoteRepository noteRepository, CancellationToken ct) =>
{
    var note = await noteRepository.GetByIdAsync(id, ct);
    return note is null ? Results.NotFound() : Results.Ok(ToResponse(note));
})
.WithName("GetNoteById");

app.MapDelete("/notes/{id:guid}", async (Guid id, INoteRepository noteRepository, CancellationToken ct) =>
{
    var deleted = await noteRepository.DeleteAsync(id, ct);
    return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteNote");

app.Run();
