namespace FactsGreet.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using FactsGreet.Services.Data;
    using FactsGreet.Services.Data.Implementations;
    using FactsGreet.Web.ViewModels.Files;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class FilesController : BaseController
    {
        private readonly IFilesService filesService;

        public FilesController(IFilesService filesService)
        {
            this.filesService = filesService;
        }

        public async Task<IActionResult> All()
        {
            return this.View(new AllFilesViewModel
            {
                Count = await this.filesService.GetCountAsync(),
                Files = await this.filesService.GetAllForUserAsync<FileViewModel>(this.UserId),
                UsedSize = await this.filesService
                    .GetUsedSizeAsync(this.UserId),
            });
        }

        public async Task<IActionResult> Delete(FileDeleteInputModel file)
        {
            // Check if the user owns the file
            if (!this.UserId.Equals(await this.filesService.GetUserIdAsync(file.Id)))
            {
                return this.Forbid();
            }

            await this.filesService.DeleteAsync(file.Id);

            return this.RedirectToAction(nameof(this.All));
        }

        public async Task<IActionResult> Rename(FileRenameInputModel file)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(this.All));
            }

            // Check if the user owns the file
            if (!this.UserId.Equals(await this.filesService.GetUserIdAsync(file.Id)))
            {
                return this.Forbid();
            }

            // Check if user already has file with that name
            if (!await this.filesService.IsFilenameAvailableAsync(file.Filename, this.UserId))
            {
                this.ViewBag.ErrorMessage = $"Filename already exists: {file.Filename}";
                return this.RedirectToAction(nameof(this.All));
            }

            await this.filesService.RenameAsync(file.Id, file.Filename);
            return this.RedirectToAction(nameof(this.All));
        }

        public IActionResult Upload()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(FileInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var filename = model.Filename ?? model.File.FileName;

            // Check if user already has file with that name
            if (!await this.filesService.IsFilenameAvailableAsync(filename, this.UserId))
            {
                this.ViewBag.ErrorMessage = $"Filename already exists: {filename}";
                return this.View(model);
            }

            try
            {
                await this.filesService.UploadAsync(
                    model.File.OpenReadStream(),
                    filename,
                    this.UserId);
            }
            catch (InvalidOperationException)
            {
                this.ViewBag.ErrorMessage = "No more available storage. Please delete some files to upload new ones";
                return this.View(model);
            }

            return this.RedirectToAction(nameof(this.All));
        }
    }
}
