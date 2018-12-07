import {Module, VuexModule, Mutation, Action, getModule} from 'vuex-module-decorators';
import store from '@/store';

export interface GlobalState {
    augurkVersion: string;
    customization: Customization | null;
}

export interface Customization {
    instanceName: string;
}

@Module({ dynamic: true, store, name: 'global'})
class Global extends VuexModule implements GlobalState {
    public augurkVersion: string = '';
    public customization: Customization | null = null;

    @Mutation
    public setCustomization(customization: Customization) {
        this.customization = customization;
    }

    @Mutation
    public setAugurkVersion(augurkVersion: string) {
        this.augurkVersion = augurkVersion;
    }

    @Action
    public async initialize() {
        // Get the version of Augurk from the backend
        const versionResult = await fetch('/api/version');
        const version = await versionResult.text();
        this.context.commit('setAugurkVersion', version);

        // Next load any customizations that have been set
        const customizationResult = await fetch('/api/v2/customization');
        const customization = await customizationResult.json();
        this.context.commit('setCustomization', customization);
    }
}

export const GlobalModule = getModule(Global);
