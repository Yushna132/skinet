// Classe pour initialisation (implémente CartType)
import { nanoid } from 'nanoid';

//En d'autre mot on creer un interface
export type CartType = {
    id: string;
    items : CartItem[];
}

export type CartItem = {
    productId: number;
    productName: string;
    price: number;
    pictureUrl: string;
    brand : string;
    type: string;
    quantity: number;
}

//Ici c'est une classe qui va heriter de l'interface
export class Cart implements CartType{
    id = nanoid(); // identifiant aléatoire unique généré côté client
    items: CartItem[] = [];
}

