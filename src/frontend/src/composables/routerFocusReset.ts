import { nextTick, onMounted } from "vue";
import { useRouter } from "vue-router";

/**
 * Reset the keyboard focus to the body tag on page changes.
 */
export const useRouterFocusReset = () => {
	onMounted(() => {
		const router = useRouter();
		router.afterEach((from, to) => {
			if (from.path !== to.path) {
				nextTick(() => {
					document.body.tabIndex = 0;
					document.body.focus();
					document.body.tabIndex = -1;
				});
			}
		});
	});
};
