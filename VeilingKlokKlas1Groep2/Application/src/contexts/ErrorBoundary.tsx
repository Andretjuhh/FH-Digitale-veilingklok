import React, {Component, ErrorInfo, ReactNode} from 'react';
import ErrorPage from "../pages/general/ErrorPage";

interface Props {
	children: ReactNode;
}

interface State {
	hasError: boolean;
	error?: Error;
}

class ErrorBoundary extends Component<Props, State> {
	constructor(props: Props) {
		super(props);
		this.state = {hasError: false};
	}

	static getDerivedStateFromError(error: Error): State {
		// Update state so the next render will show the fallback UI
		return {hasError: true, error};
	}

	componentDidCatch(error: Error, errorInfo: ErrorInfo) {
		// You can log the error to an error reporting service here
		console.error('Error caught by boundary:', error, errorInfo);
	}

	render() {
		if (this.state.hasError) {
			// Render the error page
			return <ErrorPage/>;
		}

		return this.props.children;
	}
}

export default ErrorBoundary;