export const Card = (p: any) => (
  <div className="p-4 border rounded-lg bg-white" {...p} />
);
export const CardHeader = (p: any) => <div className="mb-2" {...p} />;
export const CardTitle = (p: any) => <h2 className="font-semibold" {...p} />;
export const CardDescription = (p: any) => (
  <p className="text-sm text-gray-500" {...p} />
);
export const CardContent = (p: any) => <div {...p} />;
