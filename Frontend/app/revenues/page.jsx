import { PageLayout, PageButton, TH, Cell } from "@/components/PageCommon";
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
import { cookies } from "next/headers";
import Link from "next/link";


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
    const cookieStore = await cookies();
    const session = cookieStore.get("session")?.value;
    let error;
    let Revenues = [];
    try {
        const res = await fetch(`${process.env.NEXTJS_URL}/api/revenues`, {
            headers: {
                Cookie: `session=${session}`,
            },
            method: "GET",
            cache: 'no-store'
        });
        // if (!res.ok) throw new Error('Failed to load Revenues');
        if (res.ok) {
            Revenues = await res.json();
            console.log("res.ok Revenues", Revenues)
        }

    } catch (e) {
        console.log("!res.ok Revenues", Revenues)
        Revenues = [];
        error = "Failed to load Revenues"
    }
    return (
        <PageLayout title="Revenues" subtitle="Generate Revenues with just a click of a button">
            <PageButton href="revenues/new">Add New Revenue</PageButton>
            <div className="container mx-auto">
                {
                    !error ?
                        (<>
                            <RadioGroupDemo />
                            {
                                Revenues.length > 0 || <div className="container mx-auto">No revenue has been made</div>
                            }
                            <Table className={"max-w-2xl  border-2 border-identity-dillute/20 rounded-xl "}>
                                <TableHeader >
                                    <TableRow className={"bg-accent rounded-xl"}>
                                        <TH>Invoice ID</TH>
                                        <TH>Invoice</TH>
                                        <TH>Total Payment</TH>
                                    </TableRow>
                                </TableHeader>
                                <TableBody>
                                    {
                                        Revenues.map((Revenue, index) => (
                                            <TableRow key={index} >
                                                <Cell>
                                                    {Revenue.invoiceId}
                                                </Cell>
                                                <Cell>
                                                    <Link href={`/invoices/${Revenue.invoiceId}`}>
                                                    </Link>
                                                </Cell>
                                                <Cell>
                                                    {Revenue.total}
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
                            <Badge variant={"destructive"} className={"mx-auto"}>{error}</Badge>
                            <Table className={"container mx-auto border-2 border-identity-dillute/20 rounded-xl "}>
                                <TableHeader >
                                    <TableRow className={"bg-accent rounded-xl"}>
                                        <TH>Invoice ID</TH>
                                        <TH>Invoice</TH>
                                        <TH>Total Payment</TH>
                                    </TableRow>
                                </TableHeader>
                                <TableBody></TableBody>
                            </Table>
                        </>)
                }
            </div>
        </PageLayout >
    )
}
