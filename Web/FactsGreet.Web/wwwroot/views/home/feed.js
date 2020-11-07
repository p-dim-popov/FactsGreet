`
${this.styleLinks}
<div class="rounded border-top mb-5"> 
    <small>${this.author} ${this.isCreated ? "created" : "edited"} an article</small> 
    <div class="card">
        <div class="row no-gutters" style="transform: rotate(0)">
            <div class="col-auto">
                <img class="card-img-top" src="${this.thumbnailLink}" style="width: 200px; height: 200px" alt="">
            </div>
            <div class="col">
                <div class="card-body">
                    <h5 class="card-title">${this.title}</h5>
                    <p class="card-text">
                        ${this.shortContent}
                    </p>
                </div>
            </div>
            <a href="/Articles/${this.title.replace(' ', '_')}" class="stretched-link"></a>
        </div>
    
        <div class="card-footer w-100 text-muted">
            <div class="row">
                <div class="col-auto">
                    ${this.starsCount} ${this.starsCount === 1 ? "person" : "people"} starred this.
                </div>
                <div class="col"></div>
                <div class="col-auto">
                    <small class="text-muted">Categories: </small>
                    ${[...Array(Math.min(this.categories.length, this.categoriesDisplayCount)).keys()]
        .reduce((acc, cur) =>
            [...acc, `<a class="btn btn-primary">${this.categories[cur]}</a>`], [])
        .join('\n')}
                    ${this.categories.length > this.categoriesDisplayCount ? "..." : ""}
                </div>
            </div>
        </div>
    </div>            
</div>
`