import { Button } from "@/components/ui/button";
import {
    Table,
    TableBody,
    TableCaption,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table"
import { Badge } from "@/components/ui/badge";
import { RadioGroupDemo } from "@/components/RadioGroupDemo";


const BACKEND_URL = process.env.BACKEND_URL || "http://localhost:5000";

function TH({ children }) {
    return (
        <TableHead className={"text-center"}>
            {children}
        </TableHead>
    )
}

function Cell({ children }) {
    return (
        <TableCell className={"text-center"}>
            {children}
        </TableCell>
    )
}

function RevenueButton({ children, className, variant }) {
    variant = (variant ? "bg-" + variant : "bg-identity-dillute hover:bg-identity")
    className = variant + " " + (className || "")
    console.log("className,", className)
    return (
        <Button className={className} >
            {children}
        </Button >
    )
}

export default async function Revenues(props) {

    let Revenues = [];
    try {
        const res = await fetch(`${BACKEND_URL}/api/revenues`, {
            method: "GET",
            cache: 'no-store'
        });
        // if (!res.ok) throw new Error('Failed to load Revenues');
        if (res.ok)
            Revenues = await res.json();
    } catch (e) {
        console.error("Failed to load Revenues,", e);
        Revenues = [];
    }
    return (
        <main className="flex flex-col h-min-full w-full p-4 lg:p-0 ">
            <div className="container mx-auto my-12">
                <h1 className="text-5xl font-bold mb-8">Revenues</h1>
                <p>Generate Revenues with just a click of a button</p>
            </div>

            <div className="container mx-auto m-4">
                {/* <RadioGroup defaultValue="option-one" className="flex">
                    <div className="flex items-center space-x-2">
                        <RadioGroupItem value="option-one" id="option-one" />
                        <Label htmlFor="option-one">Option One</Label>
                    </div>
                    <div className="flex items-center space-x-2">
                        <RadioGroupItem value="option-two" id="option-two" />
                        <Label htmlFor="option-two">Option Two</Label>
                    </div>
                </RadioGroup> */}
                {/* <Link className="bg-identity object flex p-2 text-background rounded-sm w-fit" href={"Revenues/new"}>Add New Revenue</Link> */}
            </div>

            <div className="container mx-auto">
                {
                    Revenues.length > 0 ?
                        (<>
                            <RadioGroupDemo />
                            <Table className={"max-w-2xl  border-2 border-identity-dillute/20 rounded-xl "}>
                                <TableHeader >
                                    <TableRow className={"bg-accent rounded-xl"}>
                                        <TH>Invoice ID</TH>
                                        <TH>Item</TH>
                                        <TH>Total Payment</TH>
                                    </TableRow>
                                </TableHeader>
                                <TableBody>
                                    {
                                        Revenues.map((Revenue, index) => (
                                            <TableRow key={index} >
                                                <Cell>
                                                    {Revenue.InvoiceId}
                                                </Cell>
                                                <Cell>
                                                    {Revenue.Item}
                                                </Cell>
                                                <Cell>
                                                    {Revenue.Amount}
                                                </Cell>
                                            </TableRow>
                                        ))
                                    }


                                </TableBody>

                            </Table>

                            <Table className={"w-full mx-auto my-12 gap-2 flex flex-col"}>
                                <TableBody >
                                    <TableRow  >
                                        <TableCell>Total Revenue</TableCell>
                                        <TableCell>RM 1000</TableCell>
                                    </TableRow>
                                    <TableRow >
                                        <TableCell>Revenue Goal (August)</TableCell>
                                        <TableCell>RM 1000</TableCell>
                                    </TableRow>
                                    <TableRow >
                                        <TableCell>Status</TableCell>
                                        <TableCell>100%  reached</TableCell>
                                    </TableRow>
                                </TableBody>
                            </Table>
                        </>)
                        :
                        (<>
                            <Badge variant={"destructive"} className={"mx-auto"}>Failed to load Revenues</Badge>
                            <Table className={"container mx-auto border-2 border-identity-dillute/20 rounded-xl "}>
                                <TableHeader >
                                    <TableRow className={"bg-accent rounded-xl"}>
                                        <TH>Customers</TH>
                                        <TH>Status</TH>
                                        <TH>Order Date

                                        </TH>
                                        <TH>Due Date</TH>
                                        <TH>Generate Revenue</TH>
                                        <TH>Send an email</TH>
                                        <TH>Receipt</TH>
                                    </TableRow>
                                </TableHeader>
                                <TableBody></TableBody>
                            </Table>
                        </>)
                }
            </div>

        </main>
    )
}
