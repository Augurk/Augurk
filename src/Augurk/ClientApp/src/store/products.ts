import {Module, VuexModule, Mutation, Action, getModule} from 'vuex-module-decorators';
import store from '@/store';

export interface ProductsState {
    productList: Product[];
}

export interface Product {
    name: string;
    description: string;
}

@Module({ dynamic: true, store, name: 'products'})
class Products extends VuexModule implements ProductsState {
    public productList: Product[] = [];

    public get productByName() {
        return (productName: string) => this.productList.find((p) => p.name === productName);
    }

    @Mutation
    public setProducts(payload: Product[]) {
        this.productList = payload;
    }

    @Mutation
    public addProduct(payload: Product) {
        this.productList.push(payload);
    }

    @Action
    public async loadProducts() {
        const result = await fetch('/api/v3/products');
        const productList = await result.json();
        this.context.commit('setProducts', productList);
    }

    @Action
    public async ensureProductLoaded(productName: string) {
        let product = this.productList.find((p) => p.name === productName);
        if (!product) {
            const result = await fetch('/api/v3/products/' + productName);
            product = await result.json();
            if (product) {
                this.context.commit('addProduct', product);
            }
        }
    }
}

export const ProductsModule = getModule(Products);
