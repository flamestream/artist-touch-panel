import ItemType from '@/types/ItemType'

const state = {
	activeMenu: undefined,
	activeItem: undefined,
	activeAsset: undefined,
	activeOverlay: undefined,
	highlightItem: undefined,
	errorMsg: undefined
};

const getters = {
	activeAssetName(state) {

		return state.activeAsset && state.activeAsset.name;
	},
	activeFont(state) {

		let {activeAsset} = state;
		if (!activeAsset)
			return;

		if (!activeAsset.name.endsWith('.ttf'))
			return;

		return activeAsset;
	}
};

const mutations = {
	setActiveMenu(state, {id}) {

		state.activeMenu = id;
	},
	setActiveItem(state, {item}) {

		state.activeItem = item;
	},
	setActiveAsset(state, {asset}) {

		state.activeAsset = asset;
	},
	setActiveOverlay(state, {id}) {

		state.activeOverlay = id;
	},
	setErrorMsg(state, {msg}) {

		state.errorMsg = msg;
	},
	setHighlightItem(state, {item}) {

		state.highlightItem = item;
	}
};

const actions = {
	setActiveMenu({commit}, {id} = {}) {

		commit('setActiveMenu', {id});
	},
	setActiveItem({commit, rootGetters}, {id} = {}) {

		let item = rootGetters.registered({type: ItemType.name, id});
		commit('setActiveItem', {item});
	},
	setActiveAsset({commit, state, rootGetters}, {id} = {}) {

		let asset = rootGetters['assets/item']({id});
		commit('setActiveAsset', {asset});
	},
	setActiveOverlay({commit}, {id} = {}) {

		commit('setActiveOverlay', {id});
	},
	setErrorMsg({commit}, {msg} = {}) {

		commit('setErrorMsg', {msg});
		commit('setActiveOverlay', {id: 'error'});
	},
	setHighlightItem({commit, rootGetters}, {id} = {}) {

		let item = rootGetters.registered({type: ItemType.name, id});
		commit('setHighlightItem', {item});
	}
};

export default {
	namespaced: true,
	state,
	getters,
	mutations,
	actions
};
