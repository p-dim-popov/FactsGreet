namespace FactsGreet.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using FactsGreet.Services.Data;
    using FactsGreet.Web.ViewModels.Files;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class FilesController : BaseController
    {
        private readonly FilesService filesService;

        public FilesController(FilesService filesService)
        {
            this.filesService = filesService;
        }

        public async Task<IActionResult> All()
        {
            return this.View(new AllFilesViewModel
            {
                Count = await this.filesService.GetCount(),
                Files = await this.filesService.GetAllForUser<FileViewModel>(this.UserId),
                UsedSize = await this.filesService
                    .GetUsedSize(this.UserId),
            });
        }

        public async Task<IActionResult> Delete(FileDeleteInputModel file)
        {
            // Check if the user owns the file
            if (!this.UserId.Equals(await this.filesService.GetUserId(file.Id)))
            {
                return this.Unauthorized();
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
            if (!this.UserId.Equals(await this.filesService.GetUserId(file.Id)))
            {
                return this.Unauthorized();
            }

            // Check if user already has file with that name
            if (!await this.filesService.IsFilenameAvailable(file.Filename, this.UserId))
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
            if (!await this.filesService.IsFilenameAvailable(filename, this.UserId))
            {
                this.ViewBag.ErrorMessage = $"Filename already exists: {filename}";
                return this.View(model);
            }

            try
            {
                await this.filesService.UploadAsync(
                    model.File.OpenReadStream(),
                    model.File.Length,
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
