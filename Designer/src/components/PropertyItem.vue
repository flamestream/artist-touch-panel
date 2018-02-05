<template>
	<div class="item">
		<div class="label"><span>{{ label }}</span><input v-if="required" type="checkbox" v-model="defined"/></div>
		<div class="value" v-if="defined">
			<div v-if="type === 'number'">
				<input type="number" v-model="value" :min="options.min" :max="options.max" :step="options.step"/>
			</div>
			<div v-else-if="type === 'checkbox'">
				<input type="checkbox" v-model="value"/>
			</div>
			<div v-else-if="type === 'range'">
				<input type="range" v-model="value" :min="options.min" :max="options.max" :step="options.step"/>
			</div>
			<div v-else-if="type === 'rectangle'">
				<PropertyItemRectangle :obj="obj"/>
			</div>
			<div v-else-if="type === 'border'">
				<PropertyItemBorder :obj="obj"/>
			</div>
			<div v-else-if="type === 'color'">
				<PropertyItemColor :obj="obj"/>
			</div>
			<div v-else-if="type === 'option'">
				<select v-model="value">
					<option v-for="v in options.options" :key="v">{{ v }}</option>
				</select>
			</div>
			<div v-else><input type="text" v-model="value"/></div>
		</div>
	</div>
</template>

<script>
import PropertyItemRectangle from './PropertyItemRectangle'
import PropertyItemBorder from './PropertyItemBorder'
import PropertyItemColor from './PropertyItemColor'
export default {
	name: 'PropertyItem',
	components: {
		PropertyItemRectangle,
		PropertyItemBorder,
		PropertyItemColor
	},
	props: ['label', 'obj', 'type', 'options'],
	computed: {
		value: {
			get() {
				let { obj } = this;
				return obj.value;
			},
			set(value) {
				this.$store.commit({
					type: 'setValue',
					obj: this.obj,
					value
				});
			}
		},
		defined: {
			get() {
				let { obj } = this;
				return !obj.notDefined;
			},
			set(value) {
				value = !value;
				this.$store.commit({
					type: 'setDefined',
					obj: this.obj,
					value
				});
			}
		},
		required() {
			let { obj } = this;
			return !obj.required;
		}
	}
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

.item {
	background-color: #00000033;
	border-radius: 3px;
	margin-bottom: 5px;
	padding: 5px;
}

.label {
	display: flex;
}

.label > span {
	display: inline-block;
	flex: 1 1;
}

.label > input[type=checkbox] {
	vertical-align: middle;
}

.value {
	margin-top: 5px;
}

</style>