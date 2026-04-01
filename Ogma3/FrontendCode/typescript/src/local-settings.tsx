import { $id } from "@h/dom";
import { makePersisted } from "@solid-primitives/storage";
import { createEffect, createSignal } from "solid-js";
import { render } from "solid-js/web";

const LocalSettings = () => {
	const [getCollapseDeleted, setCollapseDeleted] = makePersisted(createSignal(false), {
		name: "collapse-deleted",
		storage: localStorage,
	});

	createEffect(() => {
		getCollapseDeleted();
	});

	const toggleCollapseDeleted = () => {
		setCollapseDeleted(!getCollapseDeleted());
	};

	return (
		<>
			<dt>Collapse deleted comments</dt>
			<dd>
				<button type="button" onClick={toggleCollapseDeleted} classList={{ on: getCollapseDeleted() ?? false }}>
					{getCollapseDeleted() ? "on" : "off"}
				</button>
			</dd>
		</>
	);
};

render(() => <LocalSettings />, $id("local-settings"));
