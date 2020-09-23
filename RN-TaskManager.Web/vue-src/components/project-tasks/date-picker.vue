<template>
    <div>
        <v-menu v-model="menu"
                :close-on-content-click="false"
                :nudge-right="40"
                transition="scale-transition"
                offset-y
                min-width="290px">
            <template v-slot:activator="{ on, attrs }">
                <v-text-field :value="localeFormat(value)"
                              :label="label"
                              prepend-icon="mdi-calendar-today"
                              readonly
                              clearable
                              dense
                              @click:clear="onClear"
                              v-bind="attrs"
                              v-on="on"></v-text-field>
            </template>
            <v-date-picker v-model="d" @input="onChangeDatePicker"></v-date-picker>
        </v-menu>
    </div>
</template>

<script>
    export default {
        props: ['value', 'label'],
        data () {
            return {
                menu: false,
                d: this.datePickerFormat(this.value),
            }
        },
        methods: {
            datePickerFormat(d) {

                if (d === undefined || d === null || d === '')
                    return '';

                return d.substr(0, 10);
            },
            localeFormat(d) {

                if (d === undefined || d === null || d === '')
                    return '';

                return new Date(Date.parse(d)).toLocaleDateString();
            },
            onChangeDatePicker(e) {
                this.menu = false;
                this.$emit('input', e);
            },
            onClear(e) {
                this.$emit('input', '');
            }
        }

    };
</script>

<style>

</style>