import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../core/services/shop-service';
import { Product } from '../../shared/models/product';
import { MatDialog } from '@angular/material/dialog';
import { ProductItemComponent } from '../shop/product-item-component/product-item-component';
import { FiltersDialog } from '../shop/filters-dialog/filters-dialog';
import { MatButton } from '@angular/material/button';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatIcon } from '@angular/material/icon';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { MatListOption, MatSelectionList, MatSelectionListChange } from '@angular/material/list';
import { ShopParams } from '../../shared/models/shopParams';
import { Pagination } from '../../shared/models/pagination';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-shop-component',
  imports: [
    MatButton,
    MatIcon,
    ProductItemComponent,
    MatMenu,
    MatSelectionList,
    MatListOption,
    MatMenuTrigger,
    MatPaginatorModule,
    FormsModule
  ],
  templateUrl: './shop-component.html',
  styleUrl: './shop-component.scss',
})
export class ShopComponent implements OnInit {
  private shopService = inject(ShopService);
  private dialogService = inject(MatDialog);
  /* Pricicple du MatDialog*/
  //   - Permet d’ouvrir une fenêtre modale (dialogue) au-dessus de l’écran principal.
  // - On utilise `MatDialog` pour ouvrir un composant spécifique en tant que contenu du dialogue.
  // - Le dialogue peut recevoir des données et renvoyer un résultat au composant parent.

  products?: Pagination<Product>;
  // On va remplacer ses valeur dans shopParams
  // selectedBrands: string[] = [];
  // selectedTypes: string[] = [];
  // selectedSort : string = 'name'; // défaut: alphabétique (côté API)

  shopParams = new ShopParams();
  pageSizeOptions = [5, 10, 15, 20];

  sortOptions = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price: Low-High', value: 'priceAsc' },
    { name: 'Price: High-Low', value: 'priceDesc' },
  ];

  ngOnInit(): void {
    this.initializeShop();
  }

  initializeShop() {
    this.shopService.getBrands();
    this.shopService.getTypes();
    this.getProducts();
  }

  getProducts() {
    this.shopService.getProducts(this.shopParams).subscribe({
      next: (response) => (this.products = response),
      error: (error) => console.log(error),
    });
  }

  onSearchChange(){
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }

  handlePageEvent(event: PageEvent){
    this.shopParams.pageNumber = event.pageIndex + 1;
    this.shopParams.pageSize = event.pageSize;
    this.getProducts();

  }

  onSortChange(event: MatSelectionListChange) {
    const selectedOption = event.options[0];
    if (selectedOption) {
      this.shopParams.sort = selectedOption.value;
      this.shopParams.pageNumber = 1;
      this.getProducts();
    }
  }

  openFiltersDialog() {
    const dialogRef = this.dialogService.open(FiltersDialog, {
      minWidth: '500px',
      data: {
        selectedBrands: this.shopParams.brands,
        selectedTypes: this.shopParams.types,
      },
    });

    // l’option data sert à passer des informations du composant
    // parent (ShopComponent) au composant enfant (FiltersDialog).

    // selectedBrands: this.selectedBrands
    // À gauche (selectedBrands) : c’est le nom de la propriété qui sera accessible
    // dans le dialogue via @Inject(MAT_DIALOG_DATA).

    // À droite (this.selectedBrands) : c’est la valeur actuelle dans ton ShopComponent
    // (typiquement un tableau de marques sélectionnées).

    // Ça veut dire : “Passe la variable this.selectedBrands du ShopComponent au FiltersDialog
    // sous le nom selectedBrands”.

    // afterClosed() → observable qui s’exécute quand la modale se ferme.
    dialogRef.afterClosed().subscribe({
      next: (result) => {
        if (result) {
          this.shopParams.brands = result.selectedBrands;
          this.shopParams.types = result.selectedTypes;
          this.shopParams.pageNumber = 1;
          this.getProducts();
          // this.shopService.getProducts(this.selectedBrands, this.selectedTypes).subscribe({
          //   next: (response) => (this.products = response.data),
          //   error: (error) => console.log(error),
          // });
        }
      },
    });
  }
}
